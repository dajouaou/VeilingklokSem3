using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

public class VeilingProduct
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    public int AanmeldingId { get; set; }
    public Aanmelding? Aanmelding { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public int? AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    public VeilingProductStatus Status { get; set; } = VeilingProductStatus.Queued;

    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }

    public int Hoeveelheid { get; set; }
    public int Volgorde { get; set; }

    public DateTime? ActivatedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public int? KoperId { get; set; }
    public Koper? Koper { get; set; }

    public List<Bid> Bids { get; set; } = new();
}