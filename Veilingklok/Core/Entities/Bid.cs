using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Bid
{
    public int Id { get; set; }                     // PK

    public int VeilingId { get; set; }              // FK
    public int VeilingProductId { get; set; }       // FK

    public int PlacedByGebruikerId { get; set; }    // altijd bekend (koper of veilingmeester)
    public int? KoperId { get; set; }               // gevuld als  een echte koper-bid is

    public decimal Amount { get; set; }
    public BidSource Source { get; set; } = BidSource.Buyer;
    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;

    public Veiling? Veiling { get; set; }
    public VeilingProduct? VeilingProduct { get; set; }
    public Gebruiker? PlacedByGebruiker { get; set; }
    public Koper? Koper { get; set; }
} 