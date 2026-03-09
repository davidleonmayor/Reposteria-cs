using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class SaleDetail
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(SaleId))]
    public Sale? Sale { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
}
