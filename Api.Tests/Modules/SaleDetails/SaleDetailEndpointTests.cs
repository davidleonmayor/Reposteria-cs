using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.SaleDetails;

/// <summary>
/// Pruebas funcionales del recurso SaleDetail.
/// Reglas de negocio relevantes:
/// - SaleId y ProductId deben existir al crear.
/// - Stock insuficiente impide la creación o el incremento de cantidad.
/// - Al crear, el stock del producto se descuenta.
/// - Al actualizar cantidad, el stock se ajusta por la diferencia.
/// - Al eliminar, el stock se restaura.
/// </summary>
[Collection("SaleDetailTests")]
public sealed class SaleDetailEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/saledetail";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient    _client;

    public SaleDetailEndpointTests(ApiWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    // ── IAsyncLifetime ─────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedPersonTypeAsync();  // PersonType Id=1
        await _factory.SeedCategoryAsync();    // Category  Id=1
        await _factory.SeedPersonAsync();      // Person    Id=1
        await _factory.SeedProductAsync();     // Product   Id=1, Stock=10
        await _factory.SeedSaleAsync();        // Sale      Id=1 (sin detalles)
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static object DefaultCreateBody(int saleId = 1, int productId = 1, int quantity = 2) => new
    {
        saleId,
        productId,
        quantity,
        unitPrice = (decimal?)null
    };

    private async Task<(int Id, JsonElement Json)> CreateDetailAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    private async Task<int> GetProductStockAsync(int productId = 1)
    {
        var response = await _client.GetAsync($"/api/product/{productId}");
        var json = await ParseJsonAsync(response);
        return json.GetProperty("stock").GetInt32();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/saledetail
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SD01
    /// Crear detalle válido → 201 con Product incluido.
    /// </summary>
    [Fact]
    public async Task Post_ValidDetail_Returns201WithProductIncluded()
    {
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody(quantity: 2));
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal(1, json.GetProperty("saleId").GetInt32());
        Assert.Equal(1, json.GetProperty("productId").GetInt32());
        Assert.Equal(2, json.GetProperty("quantity").GetInt32());
        Assert.True(json.GetProperty("unitPrice").GetDecimal() > 0);

        Assert.Equal(JsonValueKind.Object, json.GetProperty("product").ValueKind);
        Assert.Equal("Torta de Chocolate", json.GetProperty("product").GetProperty("name").GetString());
    }

    /// <summary>
    /// TEST-SD02
    /// Crear detalle con SaleId inexistente → 400.
    /// </summary>
    [Fact]
    public async Task Post_NonExistentSale_Returns400()
    {
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody(saleId: 99999));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SD03
    /// Crear detalle con ProductId inexistente → 400.
    /// </summary>
    [Fact]
    public async Task Post_NonExistentProduct_Returns400()
    {
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody(productId: 99999));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SD04
    /// Crear detalle con stock insuficiente → 400.
    /// </summary>
    [Fact]
    public async Task Post_InsufficientStock_Returns400()
    {
        // Product tiene Stock=10, pedimos 999
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody(quantity: 999));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SD05
    /// Crear detalle con Quantity inválido → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Post_InvalidQuantity_Returns400WithValidationErrors()
    {
        // Arrange — quantity=0 viola [Range(1, int.MaxValue)]
        var body = new { saleId = 1, productId = 1, quantity = 0, unitPrice = (decimal?)null };

        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("Quantity", out _));
    }

    /// <summary>
    /// TEST-SD06
    /// Crear detalle → el stock del producto se descuenta.
    /// </summary>
    [Fact]
    public async Task Post_ValidDetail_DeductsProductStock()
    {
        var stockBefore = await GetProductStockAsync();

        await CreateDetailAsync(DefaultCreateBody(quantity: 3));

        var stockAfter = await GetProductStockAsync();
        Assert.Equal(stockBefore - 3, stockAfter);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/saledetail
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SD07
    /// Obtener lista sin detalles → 200 con arreglo vacío.
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
    /// TEST-SD08
    /// Obtener lista con detalles → 200 con Product incluido.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingDetails_Returns200WithProductIncluded()
    {
        await CreateDetailAsync();

        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK,   response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0);

        var first = json[0];
        Assert.True(first.TryGetProperty("id",        out _));
        Assert.True(first.TryGetProperty("saleId",    out _));
        Assert.True(first.TryGetProperty("productId", out _));
        Assert.True(first.TryGetProperty("quantity",  out _));
        Assert.Equal(JsonValueKind.Object, first.GetProperty("product").ValueKind);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/saledetail/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SD09
    /// Obtener detalle por ID existente → 200 con Product incluido.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithDetail()
    {
        var (id, _) = await CreateDetailAsync();

        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id, json.GetProperty("id").GetInt32());
        Assert.Equal(JsonValueKind.Object, json.GetProperty("product").ValueKind);
    }

    /// <summary>
    /// TEST-SD10
    /// Obtener detalle por ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"{BaseUrl}/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PUT /api/saledetail/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SD11
    /// Actualizar cantidad → 204, stock ajustado por diferencia.
    /// </summary>
    [Fact]
    public async Task Put_ExistingDetail_Returns204AndAdjustsStock()
    {
        // Crea detalle con quantity=2 → stock queda en 8
        var (id, created) = await CreateDetailAsync(DefaultCreateBody(quantity: 2));
        var stockAfterCreate = await GetProductStockAsync();

        // Actualiza a quantity=5 → delta=+3, stock debe quedar en 5
        var updateBody = new { quantity = 5, unitPrice = created.GetProperty("unitPrice").GetDecimal() };
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var stockAfterUpdate = await GetProductStockAsync();
        Assert.Equal(stockAfterCreate - 3, stockAfterUpdate);
    }

    /// <summary>
    /// TEST-SD12
    /// Actualizar detalle con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Put_NonExistentId_Returns404()
    {
        var body = new { quantity = 1, unitPrice = 1000m };

        var response = await _client.PutAsJsonAsync($"{BaseUrl}/99999", body);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// TEST-SD13
    /// Aumentar cantidad más allá del stock disponible → 400.
    /// </summary>
    [Fact]
    public async Task Put_InsufficientStockOnIncrease_Returns400()
    {
        // Crea con quantity=2 → stock=8
        var (id, created) = await CreateDetailAsync(DefaultCreateBody(quantity: 2));

        // Intenta subir a quantity=999 (delta=997, stock disponible=8)
        var body = new { quantity = 999, unitPrice = created.GetProperty("unitPrice").GetDecimal() };
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DELETE /api/saledetail/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SD14
    /// Eliminar detalle existente → 204, GET retorna 404, stock restaurado.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingDetail_Returns204AndRestoresStock()
    {
        var stockBefore  = await GetProductStockAsync();
        var (id, _)      = await CreateDetailAsync(DefaultCreateBody(quantity: 2));
        var stockAfterCreate = await GetProductStockAsync();
        Assert.Equal(stockBefore - 2, stockAfterCreate);

        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // GET debe retornar 404
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Stock restaurado
        var stockAfterDelete = await GetProductStockAsync();
        Assert.Equal(stockBefore, stockAfterDelete);
    }

    /// <summary>
    /// TEST-SD15
    /// Eliminar detalle con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Delete_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
