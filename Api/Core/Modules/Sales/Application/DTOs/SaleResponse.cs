namespace Api.Core.Modules.Sales.Application.DTOs;

public sealed class SaleResponse
{
    public int Id { get; init; }
    public DateTime SaleDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Total { get; init; }
    public string State { get; init; } = string.Empty;
    public string? Observations { get; init; }
    public IReadOnlyList<SaleParticipantResponse> Participants { get; init; } = [];
    public IReadOnlyList<SaleDetailResponse> Details { get; init; } = [];

    public static SaleResponse FromEntity(global::Sale s) => new()
    {
        Id           = s.Id,
        SaleDate     = s.SaleDate,
        Subtotal     = s.Subtotal,
        Total        = s.Total,
        State        = s.State,
        Observations = s.Observations,
        Participants = s.Participants.Select(SaleParticipantResponse.FromEntity).ToList(),
        Details      = s.Details.Select(SaleDetailResponse.FromEntity).ToList()
    };
}

public sealed class SaleParticipantResponse
{
    public int Id { get; init; }
    public int PersonId { get; init; }
    public string Role { get; init; } = string.Empty;
    public PersonSummaryResponse? Person { get; init; }

    public static SaleParticipantResponse FromEntity(global::SaleParticipant p) => new()
    {
        Id       = p.Id,
        PersonId = p.PersonId,
        Role     = p.Role,
        Person   = p.Person is null ? null : PersonSummaryResponse.FromEntity(p.Person)
    };
}

public sealed class SaleDetailResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public ProductSummaryResponse? Product { get; init; }

    public static SaleDetailResponse FromEntity(global::SaleDetail d) => new()
    {
        Id        = d.Id,
        ProductId = d.ProductId,
        Quantity  = d.Quantity,
        UnitPrice = d.UnitPrice,
        Product   = d.Product is null ? null : ProductSummaryResponse.FromEntity(d.Product)
    };
}

public sealed class PersonSummaryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public static PersonSummaryResponse FromEntity(global::Person p) => new()
    {
        Id       = p.Id,
        Name     = p.Name,
        LastName = p.LastName,
        Email    = p.Email
    };
}

public sealed class ProductSummaryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }

    public static ProductSummaryResponse FromEntity(global::Product p) => new()
    {
        Id    = p.Id,
        Name  = p.Name,
        Price = p.Price
    };
}
