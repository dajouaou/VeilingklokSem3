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
    // Start veiling zonder aanmeldingen faalt
    [Fact]
    public async Task StartVeiling_GeenAanmeldingen_Faalt()
    {
        using var db = TestDb.Create();
        var s = new VeilingService(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            s.StartVeilingAsync(DateTime.Today, DateTime.Today, new TimeSpan(9, 0, 0)));
    }

    // Start veiling met aanmeldingen werkt en maakt producten
    [Fact]
    public async Task StartVeiling_MetAanmeldingen_Werkt()
    {
        using var db = TestDb.Create();

        db.Aanmeldingen.AddRange(
            TestData.Aanmelding(id: 1, leverDatum: new DateTime(2026, 1, 10), soort: "A", minPrijs: 1m),
            TestData.Aanmelding(id: 2, leverDatum: new DateTime(2026, 1, 10), soort: "B", minPrijs: 2m)
        );
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        var dto = await s.StartVeilingAsync(new DateTime(2026, 1, 11), new DateTime(2026, 1, 10), new TimeSpan(9, 0, 0));

        Assert.True(dto.Id > 0);
        Assert.Equal(2, await db.VeilingProducten.CountAsync());
    }

    // Start geplande veiling bestaat niet faalt
    [Fact]
    public async Task StartGeplandeVeiling_BestaatNiet_Faalt()
    {
        using var db = TestDb.Create();
        var s = new VeilingService(db);

        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(999));
    }

    // Start geplande veiling status fout faalt
    [Fact]
    public async Task StartGeplandeVeiling_NietGepland_Faalt()
    {
        using var db = TestDb.Create();

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gestart, DateTime.Today, new TimeSpan(9, 0, 0)));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(1));
    }

    // Start geplande veiling zonder producten faalt
    [Fact]
    public async Task StartGeplandeVeiling_GeenProducten_Faalt()
    {
        using var db = TestDb.Create();

        db.Veilingen.Add(TestData.Veiling(
            id: 1,
            status: VeilingStatus.Gepland,
            datum: DateTime.Today,
            start: DateTime.Now.TimeOfDay - TimeSpan.FromMinutes(2),
            producten: new()
        ));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => s.StartGeplandeVeilingAsync(1));
    }

    // Pause werkt alleen als gestart
    [Theory]
    [InlineData(VeilingStatus.Gepland, true)]
    [InlineData(VeilingStatus.Gepauzeerd, true)]
    [InlineData(VeilingStatus.Gestart, false)]
    public async Task Pause_StatusCheck(VeilingStatus status, bool moetFalen)
    {
        using var db = TestDb.Create();
        db.Veilingen.Add(TestData.Veiling(1, status, DateTime.Today, new TimeSpan(9, 0, 0)));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);

        if (moetFalen)
            await Assert.ThrowsAsync<ArgumentException>(() => s.PauseAsync(1));
        else
            await s.PauseAsync(1);
    }

    // Resume werkt alleen als gepauzeerd
    [Theory]
    [InlineData(VeilingStatus.Gestart, true)]
    [InlineData(VeilingStatus.Gepland, true)]
    [InlineData(VeilingStatus.Gepauzeerd, false)]
    public async Task Resume_StatusCheck(VeilingStatus status, bool moetFalen)
    {
        using var db = TestDb.Create();

        var p = new VeilingProduct { Id = 10, Volgorde = 1 };
        db.Veilingen.Add(TestData.Veiling(1, status, DateTime.Today, new TimeSpan(9, 0, 0), huidigProductId: 10, producten: new() { p }));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);

        if (moetFalen)
            await Assert.ThrowsAsync<ArgumentException>(() => s.ResumeAsync(1));
        else
        {
            await s.ResumeAsync(1);
            var prod = await db.VeilingProducten.FindAsync(10);
            Assert.NotNull(prod!.LaatstePrijsUpdateUtc);
        }
    }

    // Plaats bod faalt als koper profiel mist
    [Fact]
    public async Task PlaatsBod_GeenKoperProfiel_Faalt()
    {
        using var db = TestDb.Create();

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gestart, DateTime.Today, new TimeSpan(9, 0, 0), producten: new()));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 1, Prijs = 1, Aantal = 1 }, 10));
    }

    // Plaats bod faalt als veiling niet gestart is
    [Fact]
    public async Task PlaatsBod_VeilingNietGestart_Faalt()
    {
        using var db = TestDb.Create();

        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = TestData.Aanmelding(id: 1);
        var p = TestData.Product(100, 1, a, actief: true);

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gepauzeerd, DateTime.Today, new TimeSpan(9, 0, 0), huidigProductId: 100, producten: new() { p }));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5, Aantal = 1 }, 10));
    }

    // Plaats bod faalt als product niet actief is
    [Fact]
    public async Task PlaatsBod_ProductNietActief_Faalt()
    {
        using var db = TestDb.Create();

        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = TestData.Aanmelding(id: 1);
        var p = TestData.Product(100, 1, a, actief: false);

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gestart, DateTime.Today, new TimeSpan(9, 0, 0), huidigProductId: 100, producten: new() { p }));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5, Aantal = 1 }, 10));
    }

    // Plaats bod werkt bij deel aankoop (rest > 0)
    [Fact]
    public async Task PlaatsBod_DeelAankoop_Werkt_ResetPrijs()
    {
        using var db = TestDb.Create();

        db.Gebruikers.Add(new Gebruiker { Id = 10, Voornaam = "K", Achternaam = "U" });
        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a = TestData.Aanmelding(id: 1, hoeveelheid: 10);
        var p = TestData.Product(100, 1, a, actief: true, rest: 10, max: 6m, huidige: 4m);

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gestart, DateTime.Today, new TimeSpan(9, 0, 0), huidigProductId: 100, producten: new() { p }));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        var bod = await s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 4m, Aantal = 3 }, 10);

        var prod = await db.VeilingProducten.FindAsync(100);
        Assert.Equal(7, prod!.ResterendeHoeveelheid);
        Assert.Equal(prod.MaximumPrijs, prod.HuidigePrijs);
        Assert.NotNull(prod.LaatstePrijsUpdateUtc);
        Assert.Equal(5, prod.KoperId);
        Assert.Equal("K U", bod.KoperNaam);
    }

    // Plaats bod werkt bij volle aankoop en activeert volgende
    [Fact]
    public async Task PlaatsBod_VolAankoop_Werkt_NextActief()
    {
        using var db = TestDb.Create();

        db.Kopers.Add(new Koper { Id = 5, GebruikerId = 10 });

        var a1 = TestData.Aanmelding(id: 1, hoeveelheid: 2, soort: "A");
        var a2 = TestData.Aanmelding(id: 2, hoeveelheid: 5, soort: "B");

        var p1 = TestData.Product(100, 1, a1, actief: true, rest: 2);
        var p2 = TestData.Product(200, 2, a2, actief: false, rest: 5);

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gestart, DateTime.Today, new TimeSpan(9, 0, 0), huidigProductId: 100, producten: new() { p1, p2 }));
        await db.SaveChangesAsync();

        var s = new VeilingService(db);
        await s.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 100, Prijs = 5m, Aantal = 2 }, 10);

        var v = await db.Veilingen.Include(x => x.Producten).FirstAsync(x => x.Id == 1);
        Assert.Equal(200, v.HuidigProductId);

        var next = v.Producten.Single(x => x.Id == 200);
        Assert.True(next.IsActief);
    }
}
