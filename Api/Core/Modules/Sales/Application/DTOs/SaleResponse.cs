namespace Api.Core.Modules.Sales.Application.DTOs;

public class SaleDetailItemResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SaleResponse
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string State { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public List<SaleDetailItemResponse> Details { get; set; } = new();

    public static SaleResponse FromEntity(Sale sale) => new()
    {
        Id           = sale.Id,
        SaleDate     = sale.SaleDate,
        State        = sale.State,
        Observations = sale.Observations,
        Subtotal     = sale.Subtotal,
        Total        = sale.Total,
        Details      = sale.Details.Select(d => new SaleDetailItemResponse
        {
            Id          = d.Id,
            ProductId   = d.ProductId,
            ProductName = d.Product?.Name ?? string.Empty,
            Quantity    = d.Quantity,
            UnitPrice   = d.UnitPrice
        }).ToList()
    };
}
