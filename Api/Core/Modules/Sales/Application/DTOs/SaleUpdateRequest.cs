using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Sales.Application.DTOs;

public sealed class SaleUpdateRequest
{
    [Required, MaxLength(40)]
    public string State { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Observations { get; set; }
}
