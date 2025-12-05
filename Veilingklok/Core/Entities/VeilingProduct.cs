using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class VeilingProduct
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int? AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    public int Volgorde { get; set; }
    public int Hoeveelheid { get; set; } = 1;

    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }

    public VeilingProductStatus Status { get; set; } = VeilingProductStatus.Queued;

    public DateTime? ActivatedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public int? SoldToKoperId { get; set; }
    public Koper? SoldToKoper { get; set; }

    public List<Bid> Bids { get; set; } = new();
}
 
