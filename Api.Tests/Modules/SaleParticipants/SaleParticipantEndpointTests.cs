using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.SaleParticipants;

/// <summary>
/// Pruebas funcionales del recurso SaleParticipant.
/// Reglas de negocio relevantes:
/// - SaleId y PersonId deben existir al crear.
/// - La combinación (SaleId, PersonId, Role) es única → duplicado retorna 409.
/// - Al actualizar sólo cambia el Role; SaleId y PersonId son inmutables.
/// </summary>
[Collection("SaleParticipantTests")]
public sealed class SaleParticipantEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/saleparticipant";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient    _client;

    public SaleParticipantEndpointTests(ApiWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    // ── IAsyncLifetime ─────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedPersonTypeAsync();  // PersonType Id=1
        await _factory.SeedCategoryAsync();    // Category  Id=1  (requerido por Person indirectamente no, pero consistencia)
        await _factory.SeedPersonAsync();      // Person    Id=1
        await _factory.SeedBareSaleAsync();    // Sale      Id=1  (sin participantes)
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static object DefaultCreateBody(int saleId = 1, int personId = 1, string role = "Vendedor") => new
    {
        saleId,
        personId,
        role
    };

    private async Task<(int Id, JsonElement Json)> CreateParticipantAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/saleparticipant
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SP01
    /// Crear participante válido → 201 con Person incluida.
    /// </summary>
    [Fact]
    public async Task Post_ValidParticipant_Returns201WithPersonIncluded()
    {
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody());
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal(1,          json.GetProperty("saleId").GetInt32());
        Assert.Equal(1,          json.GetProperty("personId").GetInt32());
        Assert.Equal("Vendedor", json.GetProperty("role").GetString());

        Assert.Equal(JsonValueKind.Object, json.GetProperty("person").ValueKind);
        Assert.Equal("Juan", json.GetProperty("person").GetProperty("name").GetString());
    }

    /// <summary>
    /// TEST-SP02
    /// Crear participante con SaleId inexistente → 400.
    /// </summary>
    [Fact]
    public async Task Post_NonExistentSale_Returns400()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody(saleId: 99999));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SP03
    /// Crear participante con PersonId inexistente → 400.
    /// </summary>
    [Fact]
    public async Task Post_NonExistentPerson_Returns400()
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody(personId: 99999));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SP04
    /// Crear participante duplicado (misma combinación SaleId+PersonId+Role) → 409 Conflict.
    /// </summary>
    [Fact]
    public async Task Post_DuplicateCombination_Returns409()
    {
        // Arrange — primer registro exitoso
        await CreateParticipantAsync();

        // Act — misma combinación
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody());

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    /// <summary>
    /// TEST-SP05
    /// Crear participante con Role vacío → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Post_EmptyRole_Returns400WithValidationErrors()
    {
        var body = new { saleId = 1, personId = 1, role = "" };

        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("Role", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/saleparticipant
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SP06
    /// Obtener lista sin participantes → 200 con arreglo vacío.
    /// </summary>
    [Fact]
    public async Task GetAll_EmptyDatabase_Returns200WithEmptyArray()
    {
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK,   response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.Equal(0, json.GetArrayLength());
    }

    /// <summary>
    /// TEST-SP07
    /// Obtener lista con datos → 200 con Person incluida en cada elemento.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingParticipants_Returns200WithPersonIncluded()
    {
        await CreateParticipantAsync();

        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK,   response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0);

        var first = json[0];
        Assert.True(first.TryGetProperty("id",       out _));
        Assert.True(first.TryGetProperty("saleId",   out _));
        Assert.True(first.TryGetProperty("personId", out _));
        Assert.True(first.TryGetProperty("role",     out _));
        Assert.Equal(JsonValueKind.Object, first.GetProperty("person").ValueKind);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/saleparticipant/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SP08
    /// Obtener participante por ID existente → 200 con Person incluida.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithPersonIncluded()
    {
        var (id, _) = await CreateParticipantAsync();

        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id, json.GetProperty("id").GetInt32());
        Assert.Equal(JsonValueKind.Object, json.GetProperty("person").ValueKind);
    }

    /// <summary>
    /// TEST-SP09
    /// Obtener participante por ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"{BaseUrl}/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PUT /api/saleparticipant/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SP10
    /// Actualizar Role → 204 y persiste el cambio.
    /// </summary>
    [Fact]
    public async Task Put_ExistingParticipant_Returns204AndPersistsRole()
    {
        var (id, _) = await CreateParticipantAsync();

        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", new { role = "Supervisor" });
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);
        Assert.Equal("Supervisor", json.GetProperty("role").GetString());
    }

    /// <summary>
    /// TEST-SP11
    /// Actualizar participante con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Put_NonExistentId_Returns404()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/99999", new { role = "Supervisor" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// TEST-SP12
    /// Actualizar con Role vacío → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Put_EmptyRole_Returns400WithValidationErrors()
    {
        var (id, _) = await CreateParticipantAsync();

        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", new { role = "" });
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("Role", out _));
    }

    /// <summary>
    /// TEST-SP13
    /// Actualizar Role generando duplicado (SaleId+PersonId+NuevoRole ya existe) → 409.
    /// </summary>
    [Fact]
    public async Task Put_RoleCausesDuplicate_Returns409()
    {
        // Crear dos participantes con el mismo SaleId+PersonId, distinto Role
        var (id1, _) = await CreateParticipantAsync(DefaultCreateBody(role: "Vendedor"));
        var (id2, _) = await CreateParticipantAsync(DefaultCreateBody(role: "Comprador"));

        // Actualizar id2 al Role del id1 → UNIQUE constraint
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id2}", new { role = "Vendedor" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DELETE /api/saleparticipant/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SP14
    /// Eliminar participante existente → 204, GET posterior retorna 404.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingParticipant_Returns204AndRemovesResource()
    {
        var (id, _) = await CreateParticipantAsync();

        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// TEST-SP15
    /// Eliminar participante con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Delete_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
