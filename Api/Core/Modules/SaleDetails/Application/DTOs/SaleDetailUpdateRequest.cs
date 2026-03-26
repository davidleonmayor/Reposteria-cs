using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.SaleDetails.Application.DTOs;

public sealed class SaleDetailUpdateRequest
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, 1_000_000)]
    public decimal UnitPrice { get; set; }
}
