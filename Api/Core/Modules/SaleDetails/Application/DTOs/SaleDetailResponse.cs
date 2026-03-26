namespace Api.Core.Modules.SaleDetails.Application.DTOs;

public sealed class SaleDetailResponse
{
    public int Id { get; init; }
    public int SaleId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DetailProductResponse? Product { get; init; }

    public static SaleDetailResponse FromEntity(global::SaleDetail d) => new()
    {
        Id        = d.Id,
        SaleId    = d.SaleId,
        ProductId = d.ProductId,
        Quantity  = d.Quantity,
        UnitPrice = d.UnitPrice,
        Product   = d.Product is null ? null : DetailProductResponse.FromEntity(d.Product)
    };
}

public sealed class DetailProductResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }

    public static DetailProductResponse FromEntity(global::Product p) => new()
    {
        Id    = p.Id,
        Name  = p.Name,
        Price = p.Price,
        Stock = p.Stock
    };
}
