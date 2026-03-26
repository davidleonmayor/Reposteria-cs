using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Sales.Application.DTOs;

public sealed class SaleParticipantRequest
{
    [Range(1, int.MaxValue)]
    public int PersonId { get; set; }

    [Required, MaxLength(40)]
    public string Role { get; set; } = string.Empty;
}
