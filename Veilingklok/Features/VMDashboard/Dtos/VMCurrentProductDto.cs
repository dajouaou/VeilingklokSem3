// Veilingklok/Features/VM/Dtos/VMCurrentProductDto.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMCurrentProductDto
{
    public int Id { get; set; }
    public int Volgorde { get; set; }

    public int? ProductId { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }

    public int? AanvoerderId { get; set; }
    public string AanvoerderNaam { get; set; } = string.Empty;

    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }

    public VeilingProductStatus Status { get; set; }
    public DateTime? ActivatedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public List<VMBidDto> Bids { get; set; } = new();

    public static VMCurrentProductDto FromEntity(VeilingProduct p)
    {
        var bids = (p.Bids ?? new List<Bid>())
            .OrderByDescending(b => b.PlacedAtUtc)
            .Select(VMBidDto.FromEntity)
            .ToList();

        var productNaam =
            p.Product?.Naam
            ?? p.Aanmelding?.Soort
            ?? string.Empty;

        var fotoUrl =
            p.Product?.FotoUrl
            ?? p.Aanmelding?.FotoUrl;

        var aanvoerderNaam =
            p.Aanvoerder?.Naam
            ?? p.Aanmelding?.Aanvoerder?.Naam
            ?? string.Empty;

        return new VMCurrentProductDto
        {
            Id = p.Id,
            Volgorde = p.Volgorde,
            ProductId = p.ProductId,
            ProductNaam = productNaam,
            FotoUrl = fotoUrl,
            AanvoerderId = p.AanvoerderId,
            AanvoerderNaam = aanvoerderNaam,
            Hoeveelheid = p.Hoeveelheid,
            StartPrijs = p.StartPrijs,
            HuidigePrijs = p.HuidigePrijs,
            Status = p.Status,
            ActivatedAtUtc = p.ActivatedAtUtc,
            ClosedAtUtc = p.ClosedAtUtc,
            Bids = bids
        };
    }
}
