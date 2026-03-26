using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Products.Application.DTOs;

public sealed class ProductUpdateRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    // 0 = mantener el CategoryId existente
    [Range(0, int.MaxValue)]
    public int CategoryId { get; set; }

    public bool Active { get; set; }
}
