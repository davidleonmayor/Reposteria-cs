namespace Api.Core.Modules.Products.Application.DTOs;

public sealed class ProductResponse
{
    public int     Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string  Description { get; set; } = string.Empty;
    public decimal Price       { get; set; }
    public int     Stock       { get; set; }
    public int     CategoryId  { get; set; }
    public bool    Active      { get; set; }
    public Category? Category  { get; set; }

    public static ProductResponse FromEntity(global::Product product) => new()
    {
        Id          = product.Id,
        Name        = product.Name,
        Description = product.Description,
        Price       = product.Price,
        Stock       = product.Stock,
        CategoryId  = product.CategoryId,
        Active      = product.Active,
        Category    = product.Category
    };
}
