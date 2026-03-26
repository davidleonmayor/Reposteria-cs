using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.Products;

/// <summary>
/// Pruebas funcionales del recurso Product.
/// Product tiene dos reglas de negocio relevantes:
/// - CategoryId == 0 en Create → se asigna/crea categoría "Uncategorized"
/// - CategoryId == 0 en Update → se conserva el CategoryId existente
/// Ambas se validan explícitamente.
/// </summary>
[Collection("ProductTests")]
public sealed class ProductEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/product";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient    _client;

    public ProductEndpointTests(ApiWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    // ── IAsyncLifetime ─────────────────────────────────────────────────────────
    // Seed de Category (Id=1) necesario porque Product tiene FK a Category.

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedCategoryAsync();     // Category Id=1 "Tortas"
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static object DefaultCreateBody(int categoryId = 1) => new
    {
        name        = "Torta de Chocolate",
        description = "Torta húmeda de tres capas",
        price       = 45000.00m,
        stock       = 10,
        categoryId,
        active      = true
    };

    private async Task<(int Id, JsonElement Json)> CreateProductAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/product
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PR01
    /// Crear producto válido con CategoryId existente → 201 con body correcto.
    /// Verifica que Category venga incluido en la respuesta (eager load).
    /// </summary>
    [Fact]
    public async Task Post_ValidProduct_Returns201WithCategoryIncluded()
    {
        // Arrange
        var body = new
        {
            name        = "Torta de Vainilla",
            description = "Torta esponjosa de vainilla con buttercream",
            price       = 38000.00m,
            stock       = 5,
            categoryId  = 1,
            active      = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert — código HTTP y Location
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/product/", response.Headers.Location!.ToString(),
            StringComparison.OrdinalIgnoreCase);

        // Assert — campos del body
        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal("Torta de Vainilla",                      json.GetProperty("name").GetString());
        Assert.Equal("Torta esponjosa de vainilla con buttercream", json.GetProperty("description").GetString());
        Assert.Equal(38000.00m, json.GetProperty("price").GetDecimal());
        Assert.Equal(5,         json.GetProperty("stock").GetInt32());
        Assert.Equal(1,         json.GetProperty("categoryId").GetInt32());
        Assert.True(json.GetProperty("active").GetBoolean());

        // Assert — Category cargada (eager load en AddAsync)
        Assert.Equal(JsonValueKind.Object, json.GetProperty("category").ValueKind);
        Assert.Equal("Tortas", json.GetProperty("category").GetProperty("name").GetString());
    }

    /// <summary>
    /// TEST-PR02
    /// Crear producto con CategoryId == 0 → se asigna/crea "Uncategorized" automáticamente.
    /// Verifica la regla de negocio del fallback.
    /// </summary>
    [Fact]
    public async Task Post_CategoryIdZero_AssignsUncategorizedCategory()
    {
        // Arrange
        var body = new
        {
            name        = "Producto Sin Categoría",
            description = "Test del fallback Uncategorized",
            price       = 1000m,
            stock       = 1,
            categoryId  = 0,
            active      = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // CategoryId debe ser > 0 (se asignó Uncategorized)
        Assert.True(json.GetProperty("categoryId").GetInt32() > 0);

        // La categoría asignada debe llamarse "Uncategorized"
        Assert.Equal("Uncategorized",
            json.GetProperty("category").GetProperty("name").GetString());
    }

    /// <summary>
    /// TEST-PR03
    /// Crear dos productos con CategoryId == 0 → ambos comparten la misma categoría
    /// "Uncategorized" (no se crea duplicada).
    /// </summary>
    [Fact]
    public async Task Post_TwoProductsWithCategoryIdZero_ShareSameUncategorized()
    {
        // Arrange
        var body = new { name = "P1", description = "d", price = 100m, stock = 1, categoryId = 0, active = true };

        // Act
        var (_, json1) = await CreateProductAsync(body with { name = "P1" });
        var (_, json2) = await CreateProductAsync(body with { name = "P2" });

        // Assert — mismo categoryId para ambos
        var catId1 = json1.GetProperty("categoryId").GetInt32();
        var catId2 = json2.GetProperty("categoryId").GetInt32();
        Assert.Equal(catId1, catId2);
    }

    /// <summary>
    /// TEST-PR04
    /// Crear producto con campos [Required] vacíos → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Post_MissingRequiredFields_Returns400WithValidationErrors()
    {
        // Arrange
        var body = new { name = "", description = "", price = 0m, stock = 0, categoryId = 1, active = true };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = json.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Name",        out _));
        Assert.True(errors.TryGetProperty("Description", out _));
    }

    /// <summary>
    /// TEST-PR05
    /// Crear producto con Price fuera de rango [0, 1_000_000] → 400.
    /// </summary>
    [Fact]
    public async Task Post_PriceOutOfRange_Returns400()
    {
        // Arrange
        var body = new
        {
            name        = "Producto Caro",
            description = "Test de rango",
            price       = 2_000_000m,   // excede 1_000_000
            stock       = 1,
            categoryId  = 1,
            active      = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("Price", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/product
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PR06
    /// Obtener lista con datos → 200 con arreglo y Category incluida en cada elemento.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingProducts_Returns200WithCategoryIncluded()
    {
        // Arrange
        await CreateProductAsync();

        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK,   response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0);

        var first = json[0];
        Assert.True(first.TryGetProperty("id",          out _));
        Assert.True(first.TryGetProperty("name",        out _));
        Assert.True(first.TryGetProperty("price",       out _));
        Assert.True(first.TryGetProperty("stock",       out _));
        Assert.True(first.TryGetProperty("categoryId",  out _));
        Assert.Equal(JsonValueKind.Object, first.GetProperty("category").ValueKind);
    }

    /// <summary>
    /// TEST-PR07
    /// Obtener lista con BD vacía → 200 con arreglo vacío.
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
    // GET /api/product/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PR08
    /// Obtener producto por ID existente → 200 con Category incluida.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithProduct()
    {
        // Arrange
        var (id, _) = await CreateProductAsync();

        // Act
        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id,                   json.GetProperty("id").GetInt32());
        Assert.Equal("Torta de Chocolate", json.GetProperty("name").GetString());
        Assert.Equal(JsonValueKind.Object, json.GetProperty("category").ValueKind);
    }

    /// <summary>
    /// TEST-PR09
    /// Obtener producto por ID inexistente → 404.
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
    // PUT /api/product/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PR10
    /// Actualizar producto existente con datos válidos → 204.
    /// Verifica persistencia con GET posterior.
    /// </summary>
    [Fact]
    public async Task Put_ExistingProductValidData_Returns204AndPersistsChanges()
    {
        // Arrange
        var (id, _) = await CreateProductAsync();

        var updateBody = new
        {
            name        = "Torta Actualizada",
            description = "Nueva descripción",
            price       = 50000m,
            stock       = 20,
            categoryId  = 1,
            active      = false
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);

        // Assert — 204 sin body
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        Assert.Equal(0, putResponse.Content.Headers.ContentLength ?? 0);

        // Verificación posterior
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);

        Assert.Equal("Torta Actualizada", json.GetProperty("name").GetString());
        Assert.Equal("Nueva descripción", json.GetProperty("description").GetString());
        Assert.Equal(50000m,              json.GetProperty("price").GetDecimal());
        Assert.Equal(20,                  json.GetProperty("stock").GetInt32());
        Assert.False(json.GetProperty("active").GetBoolean());
    }

    /// <summary>
    /// TEST-PR11
    /// Actualizar producto con CategoryId == 0 → conserva el CategoryId original.
    /// </summary>
    [Fact]
    public async Task Put_CategoryIdZero_KeepsExistingCategory()
    {
        // Arrange — crea producto con categoryId=1
        var (id, created) = await CreateProductAsync();
        var originalCategoryId = created.GetProperty("categoryId").GetInt32();

        var updateBody = new
        {
            name        = "Mismo producto",
            description = "Sin cambiar categoría",
            price       = 10000m,
            stock       = 5,
            categoryId  = 0,    // 0 = conservar existente
            active      = true
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // Verificación — CategoryId no cambió
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);

        Assert.Equal(originalCategoryId, json.GetProperty("categoryId").GetInt32());
    }

    /// <summary>
    /// TEST-PR12
    /// Actualizar producto con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Put_NonExistentId_Returns404()
    {
        // Arrange
        var body = new { name = "X", description = "Y", price = 1m, stock = 1, categoryId = 1, active = true };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/99999", body);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// TEST-PR13
    /// Actualizar producto con datos inválidos → 400.
    /// </summary>
    [Fact]
    public async Task Put_InvalidData_Returns400WithValidationErrors()
    {
        // Arrange
        var (id, _) = await CreateProductAsync();
        var body = new { name = "", description = "", price = 0m, stock = 0, categoryId = 1, active = true };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("Name",        out _));
        Assert.True(json.GetProperty("errors").TryGetProperty("Description", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DELETE /api/product/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-PR14
    /// Eliminar producto existente → 204. GET posterior retorna 404.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingProduct_Returns204AndRemovesResource()
    {
        // Arrange
        var (id, _) = await CreateProductAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// TEST-PR15
    /// Eliminar producto con ID inexistente → 404.
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
