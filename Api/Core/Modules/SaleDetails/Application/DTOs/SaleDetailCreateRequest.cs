using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.SaleDetails.Application.DTOs;

public sealed class SaleDetailCreateRequest
{
    [Range(1, int.MaxValue)]
    public int SaleId { get; set; }

    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, 1_000_000)]
    public decimal? UnitPrice { get; set; }
}
