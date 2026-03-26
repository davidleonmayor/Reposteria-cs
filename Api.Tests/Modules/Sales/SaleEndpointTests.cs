using Api.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Api.Tests.Modules.Sales;

/// <summary>
/// Pruebas funcionales del recurso Sale.
/// Reglas de negocio relevantes:
/// - Al crear, personas y productos deben existir.
/// - Stock insuficiente impide la creación.
/// - Al crear, el stock del producto se descuenta.
/// - Al eliminar, el stock se restaura.
/// </summary>
[Collection("SaleTests")]
public sealed class SaleEndpointTests : IClassFixture<ApiWebFactory>, IAsyncLifetime
{
    private const string BaseUrl = "/api/sale";

    private readonly ApiWebFactory _factory;
    private readonly HttpClient    _client;

    public SaleEndpointTests(ApiWebFactory factory)
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
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static object DefaultCreateBody(int personId = 1, int productId = 1, int quantity = 2) => new
    {
        state        = "Pendiente",
        observations = (string?)null,
        participants = new[] { new { personId, role = "Vendedor" } },
        details      = new[] { new { productId, quantity, unitPrice = (decimal?)null } }
    };

    private async Task<(int Id, JsonElement Json)> CreateSaleAsync(object? body = null)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl, body ?? DefaultCreateBody());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return (json.GetProperty("id").GetInt32(), json);
    }

    private static async Task<JsonElement> ParseJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    private async Task<int> GetProductStockAsync(int productId)
    {
        var response = await _client.GetAsync($"/api/product/{productId}");
        var json = await ParseJsonAsync(response);
        return json.GetProperty("stock").GetInt32();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // POST /api/sale
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SA01
    /// Crear venta válida → 201 con participantes y detalles incluidos.
    /// </summary>
    [Fact]
    public async Task Post_ValidSale_Returns201WithParticipantsAndDetails()
    {
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, DefaultCreateBody());
        var json     = await ParseJsonAsync(response);

        // Assert — código HTTP y Location
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // Assert — campos del body
        Assert.True(json.GetProperty("id").GetInt32() > 0);
        Assert.Equal("Pendiente", json.GetProperty("state").GetString());
        Assert.True(json.GetProperty("subtotal").GetDecimal() > 0);

        // Assert — participantes y detalles cargados
        Assert.Equal(JsonValueKind.Array, json.GetProperty("participants").ValueKind);
        Assert.True(json.GetProperty("participants").GetArrayLength() > 0);
        Assert.Equal(JsonValueKind.Array, json.GetProperty("details").ValueKind);
        Assert.True(json.GetProperty("details").GetArrayLength() > 0);

        // Assert — navegación Person y Product incluidas
        var participant = json.GetProperty("participants")[0];
        Assert.Equal(JsonValueKind.Object, participant.GetProperty("person").ValueKind);

        var detail = json.GetProperty("details")[0];
        Assert.Equal(JsonValueKind.Object, detail.GetProperty("product").ValueKind);
    }

    /// <summary>
    /// TEST-SA02
    /// Crear venta con PersonId inexistente → 400.
    /// </summary>
    [Fact]
    public async Task Post_NonExistentPerson_Returns400()
    {
        // Arrange
        var body = new
        {
            state        = "Pendiente",
            participants = new[] { new { personId = 99999, role = "Vendedor" } },
            details      = new[] { new { productId = 1, quantity = 1, unitPrice = (decimal?)null } }
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SA03
    /// Crear venta con ProductId inexistente → 400.
    /// </summary>
    [Fact]
    public async Task Post_NonExistentProduct_Returns400()
    {
        // Arrange
        var body = new
        {
            state        = "Pendiente",
            participants = new[] { new { personId = 1, role = "Vendedor" } },
            details      = new[] { new { productId = 99999, quantity = 1, unitPrice = (decimal?)null } }
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SA04
    /// Crear venta con stock insuficiente → 400.
    /// </summary>
    [Fact]
    public async Task Post_InsufficientStock_Returns400()
    {
        // Arrange — se pide más stock del disponible (Product tiene 10)
        var body = DefaultCreateBody(quantity: 999);

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// TEST-SA05
    /// Crear venta con campos requeridos vacíos → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Post_MissingRequiredFields_Returns400WithValidationErrors()
    {
        // Arrange — state vacío, participants y details vacíos
        var body = new
        {
            state        = "",
            participants = Array.Empty<object>(),
            details      = Array.Empty<object>()
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("State", out _));
    }

    /// <summary>
    /// TEST-SA06
    /// Crear venta → el stock del producto se descuenta.
    /// </summary>
    [Fact]
    public async Task Post_ValidSale_DeductsProductStock()
    {
        // Arrange
        var stockBefore = await GetProductStockAsync(1);

        // Act
        await CreateSaleAsync(DefaultCreateBody(quantity: 3));

        // Assert
        var stockAfter = await GetProductStockAsync(1);
        Assert.Equal(stockBefore - 3, stockAfter);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/sale
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SA07
    /// Obtener lista con BD vacía de ventas → 200 con arreglo vacío.
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

    /// <summary>
    /// TEST-SA08
    /// Obtener lista con ventas → 200 con arreglo y navegación incluida.
    /// </summary>
    [Fact]
    public async Task GetAll_WithExistingSales_Returns200WithNavigationIncluded()
    {
        // Arrange
        await CreateSaleAsync();

        // Act
        var response = await _client.GetAsync(BaseUrl);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK,   response.StatusCode);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0);

        var first = json[0];
        Assert.True(first.TryGetProperty("id",           out _));
        Assert.True(first.TryGetProperty("state",        out _));
        Assert.True(first.TryGetProperty("subtotal",     out _));
        Assert.Equal(JsonValueKind.Array, first.GetProperty("participants").ValueKind);
        Assert.Equal(JsonValueKind.Array, first.GetProperty("details").ValueKind);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GET /api/sale/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SA09
    /// Obtener venta por ID existente → 200 con participantes y detalles.
    /// </summary>
    [Fact]
    public async Task GetById_ExistingId_Returns200WithSale()
    {
        // Arrange
        var (id, _) = await CreateSaleAsync();

        // Act
        var response = await _client.GetAsync($"{BaseUrl}/{id}");
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id, json.GetProperty("id").GetInt32());
        Assert.Equal(JsonValueKind.Array, json.GetProperty("participants").ValueKind);
        Assert.Equal(JsonValueKind.Array, json.GetProperty("details").ValueKind);
    }

    /// <summary>
    /// TEST-SA10
    /// Obtener venta por ID inexistente → 404.
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
    // PUT /api/sale/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SA11
    /// Actualizar venta existente → 204 y persiste cambios.
    /// </summary>
    [Fact]
    public async Task Put_ExistingValidData_Returns204AndPersistsChanges()
    {
        // Arrange
        var (id, _) = await CreateSaleAsync();
        var updateBody = new { state = "Completada", observations = "Entregada al cliente" };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateBody);

        // Assert — 204 sin body
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // Verificación posterior
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        var json        = await ParseJsonAsync(getResponse);

        Assert.Equal("Completada",           json.GetProperty("state").GetString());
        Assert.Equal("Entregada al cliente", json.GetProperty("observations").GetString());
    }

    /// <summary>
    /// TEST-SA12
    /// Actualizar venta con ID inexistente → 404.
    /// </summary>
    [Fact]
    public async Task Put_NonExistentId_Returns404()
    {
        // Arrange
        var body = new { state = "Completada", observations = (string?)null };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/99999", body);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// TEST-SA13
    /// Actualizar venta con State vacío → 400 con errores de validación.
    /// </summary>
    [Fact]
    public async Task Put_InvalidData_Returns400WithValidationErrors()
    {
        // Arrange
        var (id, _) = await CreateSaleAsync();
        var body = new { state = "", observations = (string?)null };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", body);
        var json     = await ParseJsonAsync(response);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(json.GetProperty("errors").TryGetProperty("State", out _));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DELETE /api/sale/{id}
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// TEST-SA14
    /// Eliminar venta existente → 204, GET retorna 404, stock restaurado.
    /// </summary>
    [Fact]
    public async Task Delete_ExistingSale_Returns204AndRestoresStock()
    {
        // Arrange
        var stockBefore = await GetProductStockAsync(1);
        var (id, _)     = await CreateSaleAsync(DefaultCreateBody(quantity: 2));
        var stockAfterCreate = await GetProductStockAsync(1);
        Assert.Equal(stockBefore - 2, stockAfterCreate);

        // Act
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{id}");

        // Assert — 204
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // GET debe retornar 404
        var getResponse = await _client.GetAsync($"{BaseUrl}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Stock restaurado
        var stockAfterDelete = await GetProductStockAsync(1);
        Assert.Equal(stockBefore, stockAfterDelete);
    }

    /// <summary>
    /// TEST-SA15
    /// Eliminar venta con ID inexistente → 404.
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
