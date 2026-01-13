// Veiling/VeilingServiceTests.cs
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.Veiling.Services;
using VeilingklokUnitTest.Helpers;
using Xunit;

namespace VeilingklokUnitTest.Veiling;

public class VeilingServiceTests
{
    [Fact]
    public async Task GetActieveVeilingAsync_None_ReturnsNull()
    {
        using var db = DbContextTestHelper.Create();
        var s = new VeilingService(db);

        Assert.Null(await s.GetActieveVeilingAsync());
    }

    [Fact]
    public async Task StartVeilingAsync_NoAanmeldingen_Throws()
    {
        using var db = DbContextTestHelper.Create();
        var s = new VeilingService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            s.StartVeilingAsync(DateTime.Today, DateTime.Today, new TimeSpan(9, 0, 0)));
    }

    [Fact]
    public async Task StartVeilingAsync_CreatesGeplandeVeiling_AndProducts_Defaults()
    {
        using var db = DbContextTestHelper.Create();

        db.Aanmeldingen.AddRange(
            new Aanmelding { Id = 1, LeverDatum = new DateTime(2026, 1, 10), Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m, VeilingProductId = null },
            new Aanmelding { Id = 2, LeverDatum = new DateTime(2026, 1, 10), Soort = "B", Hoeveelheid = 5, MinimumPrijs = 2m, VeilingProductId = null }
        );
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        _ = await s.StartVeilingAsync(new DateTime(2026, 1, 11), new DateTime(2026, 1, 10), new TimeSpan(9, 0, 0));

        var v = await db.Veilingen.Include(x => x.Producten).FirstAsync();
        Assert.Equal(VeilingStatus.Gepland, v.Status);
        Assert.Equal(2, v.Producten.Count);

        var first = v.Producten.OrderBy(p => p.Volgorde).First();
        Assert.Equal(first.MaximumPrijs, first.HuidigePrijs);
        Assert.True(first.MaximumPrijs > first.MinimumPrijs);
        Assert.True(first.DalingPerSeconde > 0);
        Assert.True(first.ResterendeHoeveelheid > 0);
    }

    [Fact]
    public async Task StartGeplandeVeilingAsync_Missing_Throws()
    {
        using var db = DbContextTestHelper.Create();
        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(999));
    }

    [Fact]
    public async Task StartGeplandeVeilingAsync_NotGepland_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = DateTime.Now.TimeOfDay });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(1));
    }

    [Fact]
    public async Task StartGeplandeVeilingAsync_TooEarly_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling
        {
            Id = 1,
            Status = VeilingStatus.Gepland,
            Datum = DateTime.Today.AddDays(1),
            StartTijd = new TimeSpan(9, 0, 0),
            Producten = new()
        });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(1));
    }

    [Fact]
    public async Task StartGeplandeVeilingAsync_NoProducts_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling
        {
            Id = 1,
            Status = VeilingStatus.Gepland,
            Datum = DateTime.Today,
            StartTijd = DateTime.Now.TimeOfDay - TimeSpan.FromMinutes(1),
            Producten = new()
        });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(1));
    }

    [Fact]
    public async Task StartGeplandeVeilingAsync_Starts_ActivatesFirst_ResetsToMax()
    {
        using var db = DbContextTestHelper.Create();

        var a1 = new Aanmelding { Id = 1, Soort = "A", MinimumPrijs = 2m, Hoeveelheid = 10 };
        var p1 = new VeilingProduct { Id = 10, Aanmelding = a1, AanmeldingId = 1, Volgorde = 1, MinimumPrijs = 2m, MaximumPrijs = 7m, HuidigePrijs = 1m };

        db.Veilingen.Add(new Core.Entities.Veiling
        {
            Id = 1,
            Status = VeilingStatus.Gepland,
            Datum = DateTime.Today,
            StartTijd = DateTime.Now.TimeOfDay - TimeSpan.FromMinutes(1),
            Producten = new List<VeilingProduct> { p1 }
        });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        _ = await s.StartGeplandeVeilingAsync(1);

        var v = await db.Veilingen.Include(x => x.Producten).FirstAsync(x => x.Id == 1);
        Assert.Equal(VeilingStatus.Gestart, v.Status);
        Assert.Equal(10, v.HuidigProductId);

        var prod = v.Producten.Single();
        Assert.True(prod.IsActief);
        Assert.Equal(prod.MaximumPrijs, prod.HuidigePrijs);
        Assert.NotNull(prod.LaatstePrijsUpdateUtc);
    }

    [Fact]
    public async Task PauseAsync_NotFound_Throws()
    {
        using var db = DbContextTestHelper.Create();
        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PauseAsync(1));
    }

    [Fact]
    public async Task PauseAsync_NotStarted_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gepland, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PauseAsync(1));
    }

    [Fact]
    public async Task PauseAsync_SetsGepauzeerd()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await s.PauseAsync(1);

        Assert.Equal(VeilingStatus.Gepauzeerd, (await db.Veilingen.FindAsync(1))!.Status);
    }

    [Fact]
    public async Task ResumeAsync_NotFound_Throws()
    {
        using var db = DbContextTestHelper.Create();
        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.ResumeAsync(1));
    }

    [Fact]
    public async Task ResumeAsync_NotPaused_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.ResumeAsync(1));
    }

    [Fact]
    public async Task ResumeAsync_SetsGestart_ResetsTimer()
    {
        using var db = DbContextTestHelper.Create();
        var p = new VeilingProduct { Id = 10, Volgorde = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling
        {
            Id = 1,
            Status = VeilingStatus.Gepauzeerd,
            Datum = DateTime.Today,
            StartTijd = new TimeSpan(9, 0, 0),
            HuidigProductId = 10,
            Producten = new List<VeilingProduct> { p }
        });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await s.ResumeAsync(1);

        var v = await db.Veilingen.Include(x => x.Producten).FirstAsync(x => x.Id == 1);
        Assert.Equal(VeilingStatus.Gestart, v.Status);
        Assert.NotNull(v.Producten.Single().LaatstePrijsUpdateUtc);
    }

    [Fact]
    public async Task StopAsync_NotFound_Throws()
    {
        using var db = DbContextTestHelper.Create();
        var s = new VeilingService(db);
        await Assert.ThrowsAsync<Exception>(() => s.StopAsync(1));
    }

    [Fact]
    public async Task StopAsync_SetsAfgesloten_AndAfgeslotenOpUtc()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await s.StopAsync(1);

        var v = await db.Veilingen.FindAsync(1);
        Assert.Equal(VeilingStatus.Afgesloten, v!.Status);
        Assert.NotNull(v.AfgeslotenOpUtc);
    }

    // -------- PlaatsBodAsync --------

    [Fact]
    public async Task PlaatsBodAsync_NoKoperProfiel_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), Producten = new() });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 1, Prijs = 1 }, 777));
    }

    [Fact]
    public async Task PlaatsBodAsync_VeilingNotStarted_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });
        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gepauzeerd, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), Producten = new() });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 1, Prijs = 1 }, 10));
    }

    [Fact]
    public async Task PlaatsBodAsync_ProductIdNotFound_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        db.Veilingen.Add(new Core.Entities.Veiling
        {
            Id = 1,
            Status = VeilingStatus.Gestart,
            Datum = DateTime.Today,
            StartTijd = new TimeSpan(9, 0, 0),
            Producten = new List<VeilingProduct>()
        });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAnyAsync<Exception>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 999, Prijs = 1, Aantal = 1 }, 10));
    }

    [Fact]
    public async Task PlaatsBodAsync_ProductNotActive_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = false, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 10, HuidigePrijs = 5, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5, Aantal = 1 }, 10));
    }

    [Fact]
    public async Task PlaatsBodAsync_ProductSold_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, IsVerkocht = true, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 10, HuidigePrijs = 5, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5, Aantal = 1 }, 10));
    }

    [Fact]
    public async Task PlaatsBodAsync_ProductDoorgedraaid_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, IsDoorgedraaid = true, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 10, HuidigePrijs = 5, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5, Aantal = 1 }, 10));
    }

    [Fact]
    public async Task PlaatsBodAsync_InvalidAantal_Throws()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 2, HuidigePrijs = 5, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), HuidigProductId = 100, Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5, Aantal = 3 }, 10));
    }

    [Fact]
    public async Task PlaatsBodAsync_PrijsLeeg_UsesHuidigePrijs()
    {
        using var db = DbContextTestHelper.Create();

        db.Gebruikers.Add(new Gebruiker { Id = 10, Voornaam = "K", Achternaam = "U" });
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 10, HuidigePrijs = 4, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), HuidigProductId = 100, Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        var bod = await s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 0m, Aantal = 1 }, 10);

        Assert.Equal(4m, bod.Prijs);
    }

    [Fact]
    public async Task PlaatsBodAsync_Aantal0_BuysAll_ClosesIfLast()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 2, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 2, HuidigePrijs = 6, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), HuidigProductId = 100, Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5m, Aantal = 0 }, 10);

        var v = await db.Veilingen.FirstAsync(x => x.Id == 1);
        Assert.Equal(VeilingStatus.Afgesloten, v.Status);
    }

    [Fact]
    public async Task PlaatsBodAsync_PartialBuy_ResetsPriceToMax_AndTimer_DecreasesQty()
    {
        using var db = DbContextTestHelper.Create();

        db.Gebruikers.Add(new Gebruiker { Id = 10, Voornaam = "K", Achternaam = "U" });
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 10, MinimumPrijs = 1m };
        var p = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, Aanmelding = a, AanmeldingId = 1, ResterendeHoeveelheid = 10, HuidigePrijs = 5, MaximumPrijs = 6, MinimumPrijs = 1 };

        db.Veilingen.Add(new Core.Entities.Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0), HuidigProductId = 100, Producten = new List<VeilingProduct> { p } });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        var bod = await s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 4m, Aantal = 3 }, 10);

        var prod = await db.VeilingProducten.FirstAsync(x => x.Id == 100);
        Assert.Equal(7, prod.ResterendeHoeveelheid);
        Assert.Equal(prod.MaximumPrijs, prod.HuidigePrijs);
        Assert.NotNull(prod.LaatstePrijsUpdateUtc);
        Assert.Equal(5, prod.KoperId);

        Assert.True(await db.Biedingen.AnyAsync());
        Assert.True(await db.Transacties.AnyAsync());
        Assert.Equal("K U", bod.KoperNaam);
    }

    [Fact]
    public async Task PlaatsBodAsync_FullBuy_ActivatesNext()
    {
        using var db = DbContextTestHelper.Create();
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a1 = new Aanmelding { Id = 1, Soort = "A", Hoeveelheid = 2, MinimumPrijs = 1m };
        var a2 = new Aanmelding { Id = 2, Soort = "B", Hoeveelheid = 5, MinimumPrijs = 2m };

        var p1 = new VeilingProduct { Id = 100, Volgorde = 1, IsActief = true, Aanmelding = a1, AanmeldingId = 1, ResterendeHoeveelheid = 2, HuidigePrijs = 6, MaximumPrijs = 6, MinimumPrijs = 1 };
        var p2 = new VeilingProduct { Id = 200, Volgorde = 2, IsActief = false, Aanmelding = a2, AanmeldingId = 2, ResterendeHoeveelheid = 5, HuidigePrijs = 7, MaximumPrijs = 7, MinimumPrijs = 2 };

        db.Veilingen.Add(new Core.Entities.Veiling
        {
            Id = 1,
            Status = VeilingStatus.Gestart,
            Datum = DateTime.Today,
            StartTijd = new TimeSpan(9, 0, 0),
            HuidigProductId = 100,
            Producten = new List<VeilingProduct> { p1, p2 }
        });
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5m, Aantal = 2 }, 10);

        var v = await db.Veilingen.Include(x => x.Producten).FirstAsync(x => x.Id == 1);
        var sold = v.Producten.Single(x => x.Id == 100);
        var next = v.Producten.Single(x => x.Id == 200);

        Assert.True(sold.IsVerkocht);
        Assert.False(sold.IsActief);

        Assert.True(next.IsActief);
        Assert.Equal(200, v.HuidigProductId);
        Assert.Equal(next.MaximumPrijs, next.HuidigePrijs);
        Assert.NotNull(next.LaatstePrijsUpdateUtc);
    }

    [Fact]
    public async Task GetVeilingDagenAsync_ReturnsDistinctOrdered()
    {
        using var db = DbContextTestHelper.Create();
        db.Veilingen.AddRange(
            new Core.Entities.Veiling { Id = 1, Datum = new DateTime(2026, 1, 10), StartTijd = new TimeSpan(9, 0, 0) },
            new Core.Entities.Veiling { Id = 2, Datum = new DateTime(2026, 1, 11), StartTijd = new TimeSpan(9, 0, 0) },
            new Core.Entities.Veiling { Id = 3, Datum = new DateTime(2026, 1, 10), StartTijd = new TimeSpan(10, 0, 0) }
        );
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        var days = await s.GetVeilingDagenAsync();

        Assert.Equal(new[] { "2026-01-10", "2026-01-11" }, days);
    }
}
