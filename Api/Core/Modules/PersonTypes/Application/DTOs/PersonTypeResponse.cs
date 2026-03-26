namespace Api.Core.Modules.PersonTypes.Application.DTOs;

public sealed class PersonTypeResponse
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static PersonTypeResponse FromEntity(global::PersonType personType) => new()
    {
        Id          = personType.Id,
        Name        = personType.Name,
        Description = personType.Description
    };
}
