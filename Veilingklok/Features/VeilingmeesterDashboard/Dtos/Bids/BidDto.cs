namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Bids;

public sealed class BidDto
{
    public int Id { get; set; }
    public int VeilingId { get; set; }
    public int VeilingProductId { get; set; }

    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime PlacedAtUtc { get; set; }

    public int? KoperId { get; set; }
    public string? KoperNaam { get; set; }
}