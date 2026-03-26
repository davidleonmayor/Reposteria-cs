using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.Persons;

/// <summary>
/// Pruebas funcionales del recurso Person.
/// Validan el comportamiento externo de los endpoints HTTP sin probar
/// lógica interna, seguridad JWT ni rendimiento.
///
/// Convenciones:
/// - Cada test llama a ResetDatabaseAsync() → estado limpio garantizado.
/// - Los tests que requieren datos existentes llaman a SeedPersonTypeAsync()
///   y luego crean personas vía HTTP (igual que un cliente real).
/// - El factory autentica automáticamente como Admin, por lo que los endpoints
///   protegidos por [Authorize] responden sin token real.
/// </summary>
[Collection("PersonTests")]
public sealed class PersonEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/person";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOpts =
        new(JsonSerializerDefaults.Web);

    public PersonEndpointTests(ApiWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    // ── IAsyncLifetime ─────────────────────────────────────────────────────────
    // InitializeAsync se ejecuta antes de CADA método de test.
    // Garantiza aislamiento completo entre tests sin levantar un factory nuevo.

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedPersonTypeAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    /// <summary>Payload por defecto para crear una persona válida.</summary>
    private static object DefaultCreateBody(string? email = null) => new
    {
        personTypeId = 1,
        name         = "Juan",
        lastName     = "Pérez",
        phone        = "3001234567",
        email        = email ?? "juan@test.com",
        address      = "Calle 10 #5-20",
        active       = true
    };

    /// <summary>
    /// Crea una persona vía POST y retorna su id junto con el JsonElement del body.
    /// Falla el test si la creación no retorna 201.
    /// </summary>
    private async Task<(int Id, JsonElement Json)> CreatePersonAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await ParseJsonAsync(response);
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content).RootElement;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/person
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-P01
    /// Crear persona válida → 201 Created con body correcto.
    /// Verifica: código HTTP, estructura del JSON, ausencia de passwordHash,
    /// presencia del header Location y que el id sea positivo.
    /// </summary>
    [Fact]
    public async Task Post_ValidPerson_Returns201WithCorrectBody()
    {
        // Arrange
        var body = new
        {
            personTypeId = 1,
            name         = "María",
            lastName     = "González",
            phone        = "3001234567",
            email        = "maria@test.com",
            address      = "Calle 10 #5-20",
            registerDate = "2026-01-15T10:00:00",
            active       = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert — código HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert — header Location
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/person/", response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase);

        // Assert — campos del body
        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal(1,            json.GetProperty("personTypeId").GetInt32());
        Assert.Equal("María",      json.GetProperty("name").GetString());
        Assert.Equal("González",   json.GetProperty("lastName").GetString());
        Assert.Equal("3001234567", json.GetProperty("phone").GetString());
        Assert.Equal("maria@test.com", json.GetProperty("email").GetString());
        Assert.Equal("Calle 10 #5-20", json.GetProperty("address").GetString());
        Assert.True(json.GetProperty("active").GetBoolean());

        // Assert — passwordHash no debe aparecer
        Assert.False(json.TryGetProperty("passwordHash", out _),
            "passwordHash no debe ser parte del response.");
    }

    /// <summary>
    /// TEST-P02
    /// Crear persona sin registerDate → el servidor asigna la fecha UTC actual.
    /// </summary>
    [Fact]
    public async Task Post_WithoutRegisterDate_ServerAssignsUtcDate()
    {
        // Arrange
        var body = DefaultCreateBody();

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var registerDate = json.GetProperty("registerDate").GetDateTime();
        Assert.NotEqual(default, registerDate);

        // La fecha asignada debe ser cercana a la fecha actual (margen de 60 segundos)
        var diff = (DateTime.UtcNow - registerDate.ToUniversalTime()).TotalSeconds;
        Assert.True(Math.Abs(diff) < 60,
            $"registerDate debería ser la fecha UTC actual, pero fue: {registerDate}");
    }

    /// <summary>
    /// TEST-P03
    /// Crear persona con campos [Required] vacíos → 400 Bad Request con errores de validación.
    /// </summary>
    [Fact]
    public async Task Post_MissingRequiredFields_Returns400WithValidationErrors()
    {
        // Arrange — name, lastName, phone, email y address vacíos
        var body = new
        {
            personTypeId = 1,
            name         = "",
            lastName     = "",
            phone        = "",
            email        = "",
            address      = "",
            active       = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(400, json.GetProperty("status").GetInt32());

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Name", out _),     "Debe reportar error en Name.");
        Assert.True(errors.TryGetProperty("LastName", out _), "Debe reportar error en LastName.");
        Assert.True(errors.TryGetProperty("Phone", out _),    "Debe reportar error en Phone.");
        Assert.True(errors.TryGetProperty("Email", out _),    "Debe reportar error en Email.");
        Assert.True(errors.TryGetProperty("Address", out _),  "Debe reportar error en Address.");
    }

    /// <summary>
    /// TEST-P04
    /// Crear persona con email de formato inválido → 400 con error en campo Email.
    /// </summary>
    [Fact]
    public async Task Post_InvalidEmailFormat_Returns400WithEmailError()
    {
        // Arrange
        var body = DefaultCreateBody(email: "esto-no-es-un-email");

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Email", out var emailErrors),
            "Debe existir error en el campo Email.");
        Assert.Contains("not a valid e-mail", emailErrors[0].GetString(),
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST-P05
    /// Crear persona con personTypeId = 0 → falla [Range(1, int.MaxValue)] → 400.
    /// </summary>
    [Fact]
    public async Task Post_PersonTypeIdZero_Returns400WithRangeError()
    {
        // Arrange
        var body = new
        {
            personTypeId = 0,           // viola [Range(1, int.MaxValue)]
            name         = "Ana",
            lastName     = "Ruiz",
            phone        = "3001231234",
            email        = "ana@test.com",
            address      = "Calle Falsa 123",
            active       = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("PersonTypeId", out _),
            "Debe reportar error en PersonTypeId.");
    }

    /// <summary>
    /// TEST-P06
    /// Crear persona con personTypeId inexistente en la BD → violación de FK →
    /// ExceptionHandlingMiddleware retorna 400 "Invalid reference".
    /// </summary>
    [Fact]
    public async Task Post_NonExistentPersonTypeId_Returns400InvalidReference()
    {
        // Arrange — 9999 está en rango válido pero no existe en PersonType
        var body = new
        {
            personTypeId = 9999,
            name         = "Pedro",
            lastName     = "Lara",
            phone        = "3005556677",
            email        = "pedro@test.com",
            address      = "Calle 99 #1-1",
            active       = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid reference", json.GetProperty("title").GetString());
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/person
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-P07
    /// Obtener lista con registros existentes → 200 con arreglo no vacío.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingPersons_Returns200WithArray()
    {
        // Arrange — crea una persona antes de consultar
        await CreatePersonAsync();

        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0, "El arreglo debe tener al menos un elemento.");

        var first = json[0];
        Assert.True(first.TryGetProperty("id", out _));
        Assert.True(first.TryGetProperty("name", out _));
        Assert.True(first.TryGetProperty("email", out _));
        Assert.False(first.TryGetProperty("passwordHash", out _),
            "passwordHash no debe aparecer en el listado.");
    }

    /// <summary>
    /// TEST-P08
    /// Obtener lista con base de datos vacía → 200 con arreglo vacío (no 404).
    /// </summary>
    [Fact]
    public async Task GetAll_EmptyDatabase_Returns200WithEmptyArray()
    {
        // Act — no se crea ninguna persona
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.Equal(0, json.GetArrayLength());
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/person/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-P09
    /// Obtener persona por ID existente → 200 con el objeto correcto.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithPerson()
    {
        // Arrange
        var (id, _) = await CreatePersonAsync();

        // Act
        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id,            json.GetProperty("id").GetInt32());
        Assert.Equal("Juan",        json.GetProperty("name").GetString());
        Assert.Equal("Pérez",       json.GetProperty("lastName").GetString());
        Assert.Equal("juan@test.com", json.GetProperty("email").GetString());
        Assert.False(json.TryGetProperty("passwordHash", out _),
            "passwordHash no debe aparecer en el response.");
    }

    /// <summary>
    /// TEST-P10
    /// Obtener persona por ID inexistente → 404 Not Found.
    /// </summary>
    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PUT /api/person/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-P11
    /// Actualizar persona existente con datos válidos → 204 No Content.
    /// Verifica que los cambios se persistan haciendo un GET posterior.
    /// </summary>
    [Fact]
    public async Task Put_ExistingPersonValidData_Returns204AndPersistsChanges()
    {
        // Arrange
        var (id, _) = await CreatePersonAsync();

        var updateBody = new
        {
            personTypeId = 1,
            name         = "Juan Actualizado",
            lastName     = "Pérez Nuevo",
            phone        = "3009999999",
            email        = "juan.nuevo@test.com",
            address      = "Nueva Dirección 456",
            registerDate = "2026-01-15T10:00:00Z",
            active       = false
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);

        // Assert — 204 sin body
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        Assert.Equal(0, putResponse.Content.Headers.ContentLength ?? 0);

        // Verificación posterior: los datos deben reflejar los cambios
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);

        Assert.Equal("Juan Actualizado",    json.GetProperty("name").GetString());
        Assert.Equal("Pérez Nuevo",         json.GetProperty("lastName").GetString());
        Assert.Equal("3009999999",          json.GetProperty("phone").GetString());
        Assert.Equal("juan.nuevo@test.com", json.GetProperty("email").GetString());
        Assert.Equal("Nueva Dirección 456", json.GetProperty("address").GetString());
        Assert.False(json.GetProperty("active").GetBoolean());
    }

    /// <summary>
    /// TEST-P12
    /// Actualizar persona con ID inexistente → 404 Not Found.
    /// </summary>
    [Fact]
    public async Task Put_NonExistentId_Returns404()
    {
        // Arrange
        var body = new
        {
            personTypeId = 1,
            name         = "Fantasma",
            lastName     = "Inexistente",
            phone        = "3000000000",
            email        = "ghost@test.com",
            address      = "Ninguna",
            registerDate = "2026-01-01T00:00:00Z",
            active       = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/99999", body);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// TEST-P13
    /// Actualizar persona con datos inválidos → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Put_InvalidData_Returns400WithValidationErrors()
    {
        // Arrange
        var (id, _) = await CreatePersonAsync();

        var invalidBody = new
        {
            personTypeId = 0,               // viola [Range(1, int.MaxValue)]
            name         = "",              // viola [Required]
            lastName     = "",
            phone        = "",
            email        = "no-es-email",   // viola [EmailAddress]
            address      = "",
            registerDate = "2026-01-01T00:00:00Z",
            active       = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", invalidBody);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("PersonTypeId", out _));
        Assert.True(errors.TryGetProperty("Name", out _));
        Assert.True(errors.TryGetProperty("LastName", out _));
        Assert.True(errors.TryGetProperty("Email", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DELETE /api/person/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-P14
    /// Eliminar persona existente → 204 No Content.
    /// Verifica que ya no sea accesible mediante GET posterior.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingPerson_Returns204AndRemovesResource()
    {
        // Arrange
        var (id, _) = await CreatePersonAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");

        // Assert — 204 sin body
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verificación posterior: el recurso ya no debe existir
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// TEST-P15
    /// Eliminar persona con ID inexistente → 404 Not Found.
    /// </summary>
    [Fact]
    public async Task Delete_NonExistentId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
