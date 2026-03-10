public class Sale
{
    public int Id { get; set; }

    public DateTime SaleDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public required string State { get; set; }
    public string? Observations { get; set; }

    public ICollection<SaleParticipant> Participants { get; set; } = new List<SaleParticipant>();
    public ICollection<SaleDetail> Details { get; set; } = new List<SaleDetail>();
}
