using System.Linq;
using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Veiling.Dtos;

public static class PublicMappings
{
    public static PublicProductDto ToPublicDto(this VeilingProduct vp)
        => new()
        {
            VeilingProductId = vp.Id,
            Soort = vp.Aanmelding?.Soort ?? vp.Product?.Naam ?? string.Empty,
            FotoUrl = vp.Aanmelding?.FotoUrl,
            StartPrijs = vp.StartPrijs,
            HuidigePrijs = vp.HuidigePrijs,
            Hoeveelheid = vp.Aanmelding?.Hoeveelheid ?? vp.Hoeveelheid
        };

    public static BodDto ToPublicDto(this Bid bid)
        => new()
        {
            Id = bid.Id,
            Prijs = bid.Amount,
            KoperNaam = bid.Koper?.Gebruiker == null
                ? null
                : $"{bid.Koper.Gebruiker.Voornaam} {bid.Koper.Gebruiker.Achternaam}".Trim(),
            Tijdstip = bid.PlacedAtUtc
        };
}