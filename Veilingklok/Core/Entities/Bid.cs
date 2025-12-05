using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Bid
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    public int VeilingProductId { get; set; }
    public VeilingProduct? VeilingProduct { get; set; }

    public int PlacedByGebruikerId { get; set; }
    public Gebruiker? PlacedByGebruiker { get; set; }

    public int? KoperId { get; set; }
    public Koper? Koper { get; set; }

    public decimal Amount { get; set; }
    public BidSource Source { get; set; } = BidSource.Buyer;

    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;
}
