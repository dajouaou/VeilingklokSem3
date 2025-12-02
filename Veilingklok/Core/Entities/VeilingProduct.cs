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

    public string ProductNaamSnapshot { get; set; } = string.Empty;
    public string? FotoUrlSnapshot { get; set; }
    public string? CategorieSnapshot { get; set; }
    public string? KleurSnapshot { get; set; }
    public string? HoogteSnapshot { get; set; }
    public int? AantalPerBosSnapshot { get; set; }

    public int Volgorde { get; set; }
    public int Hoeveelheid { get; set; }

    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public decimal MinimumPrijs { get; set; }

    public decimal? LaatsteBodBedrag { get; set; }
    public DateTime? LaatsteBodTijdUtc { get; set; }
    public int BiedCount { get; set; }

    public VeilingProductStatus Status { get; set; } = VeilingProductStatus.InQueue;

    public DateTime? ActivatedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public bool IsHandmatigGestopt { get; set; }
    public string? StatusReason { get; set; }

    public int ClockDurationMs { get; set; } = 10000;
    public int ClockTickMs { get; set; } = 100;
    public decimal PriceDropPerTick { get; set; } = 0.05m;

    public int? SoldToKoperId { get; set; }
    public Koper? SoldToKoper { get; set; }

    public List<Bid> Bids { get; set; } = new();

    public byte[]? RowVersion { get; set; }
}