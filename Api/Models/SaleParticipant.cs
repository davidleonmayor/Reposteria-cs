using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class SaleParticipant
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public int PersonId { get; set; }
    public required string Role { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(SaleId))]
    public Sale? Sale { get; set; }

    [ForeignKey(nameof(PersonId))]
    public Person? Person { get; set; }
}
