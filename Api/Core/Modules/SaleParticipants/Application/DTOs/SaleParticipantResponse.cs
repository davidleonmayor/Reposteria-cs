namespace Api.Core.Modules.SaleParticipants.Application.DTOs;

public sealed class SaleParticipantResponse
{
    public int Id { get; init; }
    public int SaleId { get; init; }
    public int PersonId { get; init; }
    public string Role { get; init; } = string.Empty;
    public ParticipantPersonResponse? Person { get; init; }

    public static SaleParticipantResponse FromEntity(global::SaleParticipant p) => new()
    {
        Id       = p.Id,
        SaleId   = p.SaleId,
        PersonId = p.PersonId,
        Role     = p.Role,
        Person   = p.Person is null ? null : ParticipantPersonResponse.FromEntity(p.Person)
    };
}

public sealed class ParticipantPersonResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public static ParticipantPersonResponse FromEntity(global::Person p) => new()
    {
        Id       = p.Id,
        Name     = p.Name,
        LastName = p.LastName,
        Email    = p.Email
    };
}
