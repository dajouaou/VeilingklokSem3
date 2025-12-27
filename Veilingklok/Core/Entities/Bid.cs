using System;

namespace Veilingklok.Core.Entities;

public class Bid
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling Veiling { get; set; } = null!;

    public int VeilingProductId { get; set; }
    public VeilingProduct VeilingProduct { get; set; } = null!;

    public int KoperId { get; set; }
    public Koper? Koper { get; set; }

    public decimal Amount { get; set; }
    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;
}