// src/Features/Veiling/Dtos/PublicVeilingDto.cs
using System;
using System.Collections.Generic;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class PublicVeilingDto
{
    public VeilingProductPublicDto? CurrentProduct { get; set; }
    public List<VeilingProductPublicDto> Queue { get; set; } = new();
    public List<BidPublicDto> Bids { get; set; } = new();

    public int TimeLeftMs { get; set; }
    public decimal CurrentPrice { get; set; }
    public int? HighestBidderId { get; set; }
}

public sealed class VeilingProductPublicDto
{
    public int Id { get; set; }
    public int Volgorde { get; set; }
    public int Hoeveelheid { get; set; }
    public string Status { get; set; } = string.Empty;

    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }

    public ProductSummaryDto? Product { get; set; }
    public AanvoerderSummaryDto? Aanvoerder { get; set; }
}

public sealed class ProductSummaryDto
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public string Soort { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string? PotmaatOfSteellengte { get; set; }
}

public sealed class AanvoerderSummaryDto
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
}

public sealed class BidPublicDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int? KoperId { get; set; }
    public string? KoperNaam { get; set; }
    public DateTime PlacedAtUtc { get; set; }
}

public sealed class BidResultDto
{
    public bool Accepted { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal NewPrice { get; set; }
    public int? HighestBidderId { get; set; }
}