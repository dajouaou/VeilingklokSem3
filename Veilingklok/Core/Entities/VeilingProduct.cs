using System;
using System.Collections.Generic;
using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public sealed class VeilingProduct
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling Veiling { get; set; } = null!;

    public int AanmeldingId { get; set; }
    public Aanmelding Aanmelding { get; set; } = null!;

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public int? AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    public VeilingProductStatus Status { get; set; } = VeilingProductStatus.Queued;

    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public decimal MinimumPrijs { get; set; }

    public int DurationSeconds { get; set; } = 20;

    public int Hoeveelheid { get; set; }
    public int Volgorde { get; set; }

    public DateTime? ActivatedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public int? KoperId { get; set; }
    public Koper? Koper { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public List<Bid> Bids { get; set; } = new();
}