using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}