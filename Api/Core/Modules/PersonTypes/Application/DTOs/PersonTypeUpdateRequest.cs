using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.PersonTypes.Application.DTOs;

public sealed class PersonTypeUpdateRequest
{
    [Required, MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Description { get; set; } = string.Empty;
}
