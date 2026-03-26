using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Products.Application.DTOs;

public sealed class ProductCreateRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    // 0 = sin categoría → el use case asigna "Uncategorized"
    [Range(0, int.MaxValue)]
    public int CategoryId { get; set; }

    public bool Active { get; set; } = true;
}
