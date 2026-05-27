using System.ComponentModel.DataAnnotations;

namespace Api.Core.Modules.Sales.Application.DTOs;

public class SaleDetailRequest
{
    [Required]
    public int ProductId { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

public class SaleParticipantRequest
{
    public int PersonId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class SaleCreateRequest
{
    public string State { get; set; } = "Completada";
    public string? Observations { get; set; }
    public List<SaleParticipantRequest> Participants { get; set; } = new();
    [Required, MinLength(1)]
    public List<SaleDetailRequest> Details { get; set; } = new();
}
