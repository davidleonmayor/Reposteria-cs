using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.SaleParticipants.Application.DTOs;

public sealed class SaleParticipantCreateRequest
{
    [Range(1, int.MaxValue)]
    public int SaleId { get; set; }

    [Range(1, int.MaxValue)]
    public int PersonId { get; set; }

    [Required, MaxLength(40)]
    public string Role { get; set; } = string.Empty;
}
