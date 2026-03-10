using System.ComponentModel.DataAnnotations;

public sealed class SaleCreateDto
{
    [Required, MaxLength(40)]
    public string State { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Observations { get; set; }

    public DateTime? SaleDate { get; set; }

    [Required, MinLength(1)]
    public List<SaleParticipantDto> Participants { get; set; } = new();

    [Required, MinLength(1)]
    public List<SaleDetailDto> Details { get; set; } = new();
}

public sealed class SaleUpdateDto
{
    [Required, MaxLength(40)]
    public string State { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Observations { get; set; }
}

public sealed class SaleParticipantDto
{
    [Range(1, int.MaxValue)]
    public int PersonId { get; set; }

    [Required, MaxLength(40)]
    public string Role { get; set; } = string.Empty;
}

public sealed class SaleDetailDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, 1_000_000)]
    public decimal? UnitPrice { get; set; }
}
