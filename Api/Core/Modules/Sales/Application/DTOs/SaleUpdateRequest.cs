namespace Api.Core.Modules.Sales.Application.DTOs;

public class SaleUpdateRequest
{
    public string State { get; set; } = string.Empty;
    public string? Observations { get; set; }
}
