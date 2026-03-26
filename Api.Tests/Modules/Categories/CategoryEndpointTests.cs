using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.Categories;

/// <summary>
/// Pruebas funcionales del recurso Category.
/// Category no tiene FKs entrantes (es entidad padre de Product),
/// por lo que no requiere seed previo de otras tablas.
/// </summary>
[Collection("CategoryTests")]
public sealed class CategoryEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/category";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient    _client;

    public CategoryEndpointTests(ApiWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    // ── IAsyncLifetime — reset antes de cada test ──────────────────────────────

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static object DefaultCreateBody(string? name = null) => new
    {
        name        = name ?? "Tortas",
        description = "Pasteles y tortas de todo tipo"
    };

    private async Task<(int Id, JsonElement Json)> CreateCategoryAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content).RootElement;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/category
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-C01
    /// Crear categoría válida → 201 Created con body correcto y header Location.
    /// </summary>
    [Fact]
    public async Task Post_ValidCategory_Returns201WithCorrectBody()
    {
        // Arrange
        var body = new
        {
            name        = "Galletas",
            description = "Galletas artesanales de mantequilla"
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert — código HTTP
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert — header Location apunta al recurso creado
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/category/", response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase);

        // Assert — campos del body
        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal("Galletas",                          json.GetProperty("name").GetString());
        Assert.Equal("Galletas artesanales de mantequilla", json.GetProperty("description").GetString());

        // Assert — Products no debe aparecer (JsonIgnore en entidad, excluido del DTO)
        Assert.False(json.TryGetProperty("products", out _),
            "La colección Products no debe estar en el response.");
    }

    /// <summary>
    /// TEST-C02
    /// Crear categoría con campos [Required] vacíos → 400 con errores de validación.
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
    /// TEST-C03
    /// Crear categoría con Name que excede MaxLength(100) → 400.
    /// </summary>
    [Fact]
    public async Task Post_NameExceedsMaxLength_Returns400()
    {
        // Arrange — 101 caracteres
        var body = new
        {
            name        = new string('A', 101),
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
    // GET /api/category
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-C04
    /// Obtener lista con datos → 200 con arreglo no vacío.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingCategories_Returns200WithArray()
    {
        // Arrange
        await CreateCategoryAsync();

        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0);

        var first = json[0];
        Assert.True(first.TryGetProperty("id",          out _));
        Assert.True(first.TryGetProperty("name",        out _));
        Assert.True(first.TryGetProperty("description", out _));
        Assert.False(first.TryGetProperty("products",   out _),
            "Products no debe aparecer en el listado.");
    }

    /// <summary>
    /// TEST-C05
    /// Obtener lista con BD vacía → 200 con arreglo vacío (no 404).
    /// </summary>
    [Fact]
    public async Task GetAll_EmptyDatabase_Returns200WithEmptyArray()
    {
        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK,       response.StatusCode);
        Assert.Equal(JsonValueKind.Array,     json.ValueKind);
        Assert.Equal(0, json.GetArrayLength());
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/category/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-C06
    /// Obtener categoría por ID existente → 200 con el objeto correcto.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithCategory()
    {
        // Arrange
        var (id, _) = await CreateCategoryAsync();

        // Act
        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id,       json.GetProperty("id").GetInt32());
        Assert.Equal("Tortas", json.GetProperty("name").GetString());
        Assert.False(json.TryGetProperty("products", out _));
    }

    /// <summary>
    /// TEST-C07
    /// Obtener categoría por ID inexistente → 404 Not Found.
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
    // PUT /api/category/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-C08
    /// Actualizar categoría existente con datos válidos → 204.
    /// Verifica persistencia con GET posterior.
    /// </summary>
    [Fact]
    public async Task Put_ExistingCategoryValidData_Returns204AndPersistsChanges()
    {
        // Arrange
        var (id, _) = await CreateCategoryAsync();

        var updateBody = new
        {
            name        = "Tortas Actualizadas",
            description = "Nueva descripción de tortas"
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);

        // Assert — 204 sin body
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        Assert.Equal(0, putResponse.Content.Headers.ContentLength ?? 0);

        // Verificación posterior
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);

        Assert.Equal("Tortas Actualizadas",      json.GetProperty("name").GetString());
        Assert.Equal("Nueva descripción de tortas", json.GetProperty("description").GetString());
    }

    /// <summary>
    /// TEST-C09
    /// Actualizar categoría con ID inexistente → 404.
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
    /// TEST-C10
    /// Actualizar categoría con datos inválidos → 400.
    /// </summary>
    [Fact]
    public async Task Put_InvalidData_Returns400WithValidationErrors()
    {
        // Arrange
        var (id, _) = await CreateCategoryAsync();
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
    // DELETE /api/category/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-C11
    /// Eliminar categoría existente → 204. Verifica que GET posterior retorne 404.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingCategory_Returns204AndRemovesResource()
    {
        // Arrange
        var (id, _) = await CreateCategoryAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verificación posterior
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// TEST-C12
    /// Eliminar categoría con ID inexistente → 404.
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
