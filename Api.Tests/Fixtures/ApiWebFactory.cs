using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Fixtures;

/// <summary>
/// Factory que levanta la Web API en memoria para pruebas funcionales.
///
/// Decisiones de infraestructura:
/// - SQLite in-memory con conexión compartida: preserva el comportamiento real de EF Core
///   (constraints de FK, cascade delete, etc.) sin tocar la base de datos de desarrollo.
/// - La conexión se mantiene abierta durante toda la vida del factory; SQLite in-memory
///   destruye la base de datos cuando no hay conexiones abiertas.
/// - TestAuthHandler reemplaza JWT Bearer para que los endpoints protegidos sean
///   accesibles sin tokens reales — las pruebas se enfocan en comportamiento funcional.
/// </summary>
public sealed class ApiWebFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Abre la conexión aquí; se mantiene abierta hasta Dispose()
        // para que SQLite no destruya la base de datos en memoria.
        _connection.Open();

        builder.ConfigureAppConfiguration((_, config) =>
        {
            // Configuración mínima para que Program.cs no falle al leer Jwt:Key
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-key-minimum-32-characters-long-for-hmac!!",
                ["Jwt:Issuer"] = "Api",
                ["Jwt:Audience"] = "ApiUsers",
                ["Jwt:ExpirationHours"] = "1"
            });
        });

        builder.ConfigureServices(services =>
        {
            // ── Base de datos ──────────────────────────────────────────────────────
            // Remueve el DbContext registrado con la conexión real (app.db)
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            // Registra el DbContext apuntando a la conexión SQLite in-memory
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            // ── Autenticación ──────────────────────────────────────────────────────
            // Reemplaza el esquema por defecto (JwtBearer) con el handler de prueba
            services.Configure<AuthenticationOptions>(opts =>
            {
                opts.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                opts.DefaultChallengeScheme    = TestAuthHandler.SchemeName;
                opts.DefaultScheme             = TestAuthHandler.SchemeName;
            });

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }

    /// <summary>
    /// Crea el esquema de la base de datos y lo devuelve listo para usar en tests.
    /// Debe llamarse al inicio de cada test para garantizar un estado limpio.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // EnsureDeleted + EnsureCreated reinicia también el contador de autoincrement
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Inserta un PersonType de prueba con Id=1 y Name="Admin".
    /// Debe llamarse después de ResetDatabaseAsync() en cada test que necesite personas.
    /// </summary>
    public async Task SeedPersonTypeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.PersonType.Add(new PersonType
        {
            Name = "Admin",
            Description = "Administrador del sistema"
        });

        await db.SaveChangesAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Close();

        base.Dispose(disposing);
    }
}
