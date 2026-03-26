using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.SaleParticipants.Application.DTOs;

public sealed class SaleParticipantUpdateRequest
{
    [Required, MaxLength(40)]
    public string Role { get; set; } = string.Empty;
}
