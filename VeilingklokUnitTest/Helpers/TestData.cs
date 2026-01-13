using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace VeilingklokUnitTest.Helpers;

public static class TestData
{
    public static Aanvoerder Aanvoerder(int id = 1, string naam = "Jan")
        => new() { Id = id, Naam = naam };

    public static Aanmelding Aanmelding(
        int id = 1,
        DateTime? leverDatum = null,
        string soort = "Appel",
        int hoeveelheid = 10,
        decimal minPrijs = 1m,
        Aanvoerder? aanvoerder = null)
    {
        var a = aanvoerder ?? Aanvoerder();
        return new Aanmelding
        {
            Id = id,
            LeverDatum = leverDatum ?? DateTime.Today.AddDays(1),
            Soort = soort,
            Hoeveelheid = hoeveelheid,
            MinimumPrijs = minPrijs,
            AanvoerderId = a.Id,
            Aanvoerder = a
        };
    }

    public static VeilingProduct Product(
        int id,
        int volgorde,
        Aanmelding aanmelding,
        bool actief = false,
        bool verkocht = false,
        bool doorgedraaid = false,
        decimal min = 1m,
        decimal max = 6m,
        decimal huidige = 6m,
        decimal daling = 0.10m,
        int rest = 10)
    {
        return new VeilingProduct
        {
            Id = id,
            Volgorde = volgorde,
            Aanmelding = aanmelding,
            AanmeldingId = aanmelding.Id,
            IsActief = actief,
            IsVerkocht = verkocht,
            IsDoorgedraaid = doorgedraaid,
            MinimumPrijs = min,
            MaximumPrijs = max,
            HuidigePrijs = huidige,
            DalingPerSeconde = daling,
            ResterendeHoeveelheid = rest
        };
    }

    public static Veilingklok.Core.Entities.Veiling Veiling(
        int id,
        VeilingStatus status,
        DateTime datum,
        TimeSpan start,
        int? huidigProductId = null,
        List<VeilingProduct>? producten = null)
    {
        return new Veilingklok.Core.Entities.Veiling
        {
            Id = id,
            Status = status,
            Datum = datum.Date,
            StartTijd = start,
            HuidigProductId = huidigProductId,
            Producten = producten ?? new()
        };
    }
}
