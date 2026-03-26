using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.PersonTypes;

/// <summary>
/// Pruebas funcionales del recurso PersonType.
/// PersonType es una entidad padre (lookup) sin FKs propias.
/// Person depende de PersonType via PersonTypeId, por lo que
/// PersonType debe existir antes de crear Persons.
/// </summary>
[Collection("PersonTypeTests")]
public sealed class PersonTypeEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/persontype";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient    _client;

    public PersonTypeEndpointTests(ApiWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static object DefaultCreateBody(string? name = null) => new
    {
        name        = name ?? "Admin",
        description = "Administrador del sistema"
    };

    private async Task<(int Id, JsonElement Json)> CreatePersonTypeAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/persontype
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PT01
    /// Crear PersonType válido → 201 con body correcto y header Location.
    /// Verifica que Persons (colección JsonIgnore) no aparezca en el response.
    /// </summary>
    [Fact]
    public async Task Post_ValidPersonType_Returns201WithCorrectBody()
    {
        // Arrange
        var body = new
        {
            name        = "Seller",
            description = "Vendedor de productos"
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert — código HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert — header Location
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/persontype/", response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase);

        // Assert — campos del body
        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal("Seller",                  json.GetProperty("name").GetString());
        Assert.Equal("Vendedor de productos",   json.GetProperty("description").GetString());

        // Assert — Persons no debe aparecer (JsonIgnore en entidad, excluido del DTO)
        Assert.False(json.TryGetProperty("persons", out _),
            "La colección Persons no debe estar en el response.");
    }

    /// <summary>
    /// TEST-PT02
    /// Crear PersonType con campos [Required] vacíos → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Post_MissingRequiredFields_Returns400WithValidationErrors()
    {
        // Arrange
        var body = new { name = "", description = "" };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(400, json.GetProperty("status").GetInt32());

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Name",        out _), "Debe reportar error en Name.");
        Assert.True(errors.TryGetProperty("Description", out _), "Debe reportar error en Description.");
    }

    /// <summary>
    /// TEST-PT03
    /// Crear PersonType con Name que excede MaxLength(60) → 400.
    /// </summary>
    [Fact]
    public async Task Post_NameExceedsMaxLength_Returns400()
    {
        // Arrange — 61 caracteres
        var body = new
        {
            name        = new string('A', 61),
            description = "Descripción válida"
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("Name", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/persontype
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PT04
    /// Obtener lista con datos → 200 con arreglo no vacío.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingPersonTypes_Returns200WithArray()
    {
        // Arrange
        await CreatePersonTypeAsync();

        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK,    response.StatusCode);
        Assert.Equal(JsonValueKind.Array,  json.ValueKind);
        Assert.True(json.GetArrayLength() > 0);

        var first = json[0];
        Assert.True(first.TryGetProperty("id",          out _));
        Assert.True(first.TryGetProperty("name",        out _));
        Assert.True(first.TryGetProperty("description", out _));
        Assert.False(first.TryGetProperty("persons",    out _),
            "Persons no debe aparecer en el listado.");
    }

    /// <summary>
    /// TEST-PT05
    /// Obtener lista con BD vacía → 200 con arreglo vacío (no 404).
    /// </summary>
    [Fact]
    public async Task GetAll_EmptyDatabase_Returns200WithEmptyArray()
    {
        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK,   response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.Equal(0, json.GetArrayLength());
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/persontype/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PT06
    /// Obtener PersonType por ID existente → 200 con el objeto correcto.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithPersonType()
    {
        // Arrange
        var (id, _) = await CreatePersonTypeAsync();

        // Act
        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id,      json.GetProperty("id").GetInt32());
        Assert.Equal("Admin", json.GetProperty("name").GetString());
        Assert.False(json.TryGetProperty("persons", out _));
    }

    /// <summary>
    /// TEST-PT07
    /// Obtener PersonType por ID inexistente → 404 Not Found.
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
    // PUT /api/persontype/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PT08
    /// Actualizar PersonType existente con datos válidos → 204.
    /// Verifica persistencia con GET posterior.
    /// </summary>
    [Fact]
    public async Task Put_ExistingPersonTypeValidData_Returns204AndPersistsChanges()
    {
        // Arrange
        var (id, _) = await CreatePersonTypeAsync();

        var updateBody = new
        {
            name        = "Manager",
            description = "Gerente de área"
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);

        // Assert — 204 sin body
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        Assert.Equal(0, putResponse.Content.Headers.ContentLength ?? 0);

        // Verificación posterior
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);

        Assert.Equal("Manager",        json.GetProperty("name").GetString());
        Assert.Equal("Gerente de área", json.GetProperty("description").GetString());
    }

    /// <summary>
    /// TEST-PT09
    /// Actualizar PersonType con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Put_NonExistentId_Returns404()
    {
        // Arrange
        var body = new { name = "X", description = "Y" };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/99999", body);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// TEST-PT10
    /// Actualizar PersonType con datos inválidos → 400.
    /// </summary>
    [Fact]
    public async Task Put_InvalidData_Returns400WithValidationErrors()
    {
        // Arrange
        var (id, _) = await CreatePersonTypeAsync();
        var body    = new { name = "", description = "" };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Name",        out _));
        Assert.True(errors.TryGetProperty("Description", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DELETE /api/persontype/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PT11
    /// Eliminar PersonType existente (sin Persons asociadas) → 204.
    /// Verifica que GET posterior retorne 404.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingPersonType_Returns204AndRemovesResource()
    {
        // Arrange
        var (id, _) = await CreatePersonTypeAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verificación posterior
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// TEST-PT12
    /// Eliminar PersonType con ID inexistente → 404.
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
