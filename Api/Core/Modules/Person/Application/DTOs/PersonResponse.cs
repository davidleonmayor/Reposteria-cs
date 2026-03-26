namespace Api.Core.Modules.Persons.Application.DTOs;

public sealed class PersonResponse
{
    public int Id { get; set; }
    public int PersonTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime RegisterDate { get; set; }
    public bool Active { get; set; }
    public PersonType? PersonType { get; set; }

    public static PersonResponse FromEntity(global::Person person) => new()
    {
        Id = person.Id,
        PersonTypeId = person.PersonTypeId,
        Name = person.Name,
        LastName = person.LastName,
        Phone = person.Phone,
        Email = person.Email,
        Address = person.Address,
        RegisterDate = person.RegisterDate,
        Active = person.Active,
        PersonType = person.PersonType
    };
}
