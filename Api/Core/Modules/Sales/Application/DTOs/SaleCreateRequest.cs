using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Sales.Application.DTOs;

public sealed class SaleCreateRequest
{
    [Required, MaxLength(40)]
    public string State { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Observations { get; set; }

    public DateTime? SaleDate { get; set; }

    [Required, MinLength(1)]
    public List<SaleParticipantRequest> Participants { get; set; } = new();

    [Required, MinLength(1)]
    public List<SaleDetailRequest> Details { get; set; } = new();
}
