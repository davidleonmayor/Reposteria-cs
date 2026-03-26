namespace Api.Core.Modules.Categories.Application.DTOs;

public sealed class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static CategoryResponse FromEntity(global::Category category) => new()
    {
        Id          = category.Id,
        Name        = category.Name,
        Description = category.Description
    };
}
