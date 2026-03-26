using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Sales.Application.DTOs;

public sealed class SaleDetailRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, 1_000_000)]
    public decimal? UnitPrice { get; set; }
}
