using System.ComponentModel.DataAnnotations;

public sealed class ProductCreateDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    // If 0, controller assigns "Uncategorized"
    [Range(0, int.MaxValue)]
    public int CategoryId { get; set; }

    public bool Active { get; set; } = true;
}

public sealed class ProductUpdateDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    // If 0, keep existing CategoryId
    [Range(0, int.MaxValue)]
    public int CategoryId { get; set; }

    public bool Active { get; set; }
}
