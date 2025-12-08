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
    public int ProductId { get; set; }
    public int? AanvoerderId { get; set; }

    public string ProductNaam { get; set; } = string.Empty;
    public string Aanvoerder { get; set; } = string.Empty;

    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public int Hoeveelheid { get; set; }
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

        return new VMCurrentProductDto
        {
            Id = p.Id,
            ProductId = p.ProductId,
            AanvoerderId = p.AanvoerderId,
            ProductNaam = p.Product?.Naam ?? string.Empty,
            Aanvoerder = p.Aanvoerder?.Naam ?? string.Empty,
            StartPrijs = p.StartPrijs,
            HuidigePrijs = p.HuidigePrijs,
            Hoeveelheid = p.Hoeveelheid,
            Status = p.Status,
            ActivatedAtUtc = p.ActivatedAtUtc,
            ClosedAtUtc = p.ClosedAtUtc,
            Bids = bids
        };
    }
}