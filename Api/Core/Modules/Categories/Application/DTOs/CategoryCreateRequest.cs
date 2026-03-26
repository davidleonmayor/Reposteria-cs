using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Categories.Application.DTOs;

public sealed class CategoryCreateRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
