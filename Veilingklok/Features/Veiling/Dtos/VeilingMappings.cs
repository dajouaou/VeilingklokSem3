// src/Features/Veiling/Dtos/VeilingMappings.cs
using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Veiling.Dtos;

public static class VeilingMappings
{
    public static VeilingProductPublicDto ToPublicDto(this VeilingProduct vp)
    {
        return new VeilingProductPublicDto
        {
            Id           = vp.Id,
            Volgorde     = vp.Volgorde,
            Hoeveelheid  = vp.Hoeveelheid,
            Status       = vp.Status.ToString(),
            StartPrijs   = vp.StartPrijs,
            HuidigePrijs = vp.HuidigePrijs,
            Product = vp.Product == null
                ? null
                : new ProductSummaryDto
                {
                    Id        = vp.Product.Id,
                    Naam      = vp.Product.Naam,
                    Categorie = vp.Product.Categorie,
                    Soort     = vp.Product.Soort,
                    FotoUrl   = vp.Product.FotoUrl,
                    PotmaatOfSteellengte = vp.Product.PotmaatOfSteellengte
                },
            Aanvoerder = vp.Aanvoerder == null
                ? null
                : new AanvoerderSummaryDto
                {
                    Id   = vp.Aanvoerder.Id,
                    Naam = vp.Aanvoerder.Naam
                }
        };
    }

    public static BidPublicDto ToPublicDto(this Bid b)
    {
        return new BidPublicDto
        {
            Id          = b.Id,
            Amount      = b.Amount,
            KoperId     = b.KoperId,
            PlacedAtUtc = b.PlacedAtUtc,
            KoperNaam   = b.Koper?.Naam ?? b.PlacedByGebruiker?.FullName
        };
    }
}