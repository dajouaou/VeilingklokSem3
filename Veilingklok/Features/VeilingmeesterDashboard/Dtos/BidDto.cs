namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class BidDto
{
    public int Id { get; set; }
    public int VeilingProductId { get; set; }

    public int? KoperId { get; set; }
    public string? KoperNaam { get; set; }

    public decimal Amount { get; set; }
    public string Source { get; set; } = "";
    public DateTime PlacedAtUtc { get; set; }
}