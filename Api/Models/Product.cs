using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int Price { get; set; }
    public int Stock { get; set; }
    public bool Active { get; set; }

    //[ForeignKey(nameof(CategoriId))]
    //public int Categori? Categori { get; set; }
    [JsonIgnore]
    public ICollection<Category> Categorys { get; set; } = new List<Category>();
}