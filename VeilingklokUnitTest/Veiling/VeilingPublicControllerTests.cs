using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using VeilingklokUnitTest.Helpers;
using Xunit;

namespace VeilingklokUnitTest.Veiling;

public class VeilingPublicControllerTests
{
    // Geen aanmeldingen geeft lege lijst
    [Fact]
    public async Task GetPublicVeildagen_Leeg_ReturnsEmpty()
    {
        using var db = TestDb.Create();
        var c = new VeilingPublicController(db);

        var res = await c.GetPublicVeildagen();

        var list = Assert.IsType<List<string>>(res.Value);
        Assert.Empty(list);
    }

    // Alleen VeilingProductId null telt mee en is gesorteerd
    [Fact]
    public async Task GetPublicVeildagen_FiltertEnSorteert()
    {
        using var db = TestDb.Create();

        db.Aanmeldingen.Add(TestData.Aanmelding(id: 1, leverDatum: new DateTime(2026, 1, 11)));
        db.Aanmeldingen.Add(TestData.Aanmelding(id: 2, leverDatum: new DateTime(2026, 1, 10)));
        db.Aanmeldingen.Add(TestData.Aanmelding(id: 3, leverDatum: new DateTime(2026, 1, 10), soort: "B"));

        // Deze moet weg (heeft VeilingProductId)
        var x = TestData.Aanmelding(id: 4, leverDatum: new DateTime(2026, 1, 12));
        x.VeilingProductId = 99;
        db.Aanmeldingen.Add(x);

        await db.SaveChangesAsync();

        var c = new VeilingPublicController(db);
        var res = await c.GetPublicVeildagen();

        var list = Assert.IsType<List<string>>(res.Value);
        Assert.Equal(new[] { "2026-01-10", "2026-01-11" }, list);
    }

    // Geen actieve veiling geeft default overzicht
    [Fact]
    public async Task GetActief_GeenVeiling_DefaultDto()
    {
        using var db = TestDb.Create();
        var c = new VeilingPublicController(db);

        var result = await c.GetActief();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

        Assert.Equal(0, dto.Id);
        Assert.False(dto.IsGestart);
        Assert.False(dto.IsPauze);
        Assert.Null(dto.HuidigProduct);
        Assert.Empty(dto.Wachtrij);
    }

    // Actieve veiling met status gestart geeft huidig product en wachtrij
    [Fact]
    public async Task GetActief_Gestart_HuidigEnWachtrij()
    {
        using var db = TestDb.Create();

        var a1 = TestData.Aanmelding(id: 1, soort: "A");
        var a2 = TestData.Aanmelding(id: 2, soort: "B");

        var p1 = TestData.Product(id: 101, volgorde: 1, aanmelding: a1, actief: true, rest: 10);
        var p2 = TestData.Product(id: 102, volgorde: 2, aanmelding: a2, actief: false, rest: 5);

        db.Veilingen.Add(TestData.Veiling(
            id: 1,
            status: VeilingStatus.Gestart,
            datum: DateTime.Today,
            start: new TimeSpan(9, 0, 0),
            huidigProductId: 101,
            producten: new() { p1, p2 }
        ));

        await db.SaveChangesAsync();

        var c = new VeilingPublicController(db);
        var result = await c.GetActief();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

        Assert.True(dto.IsGestart);
        Assert.NotNull(dto.HuidigProduct);
        Assert.Single(dto.Wachtrij);
    }

    // Status gepauzeerd geeft IsPauze true
    [Fact]
    public async Task GetActief_Gepauzeerd_IsPauzeTrue()
    {
        using var db = TestDb.Create();

        db.Veilingen.Add(TestData.Veiling(
            id: 1,
            status: VeilingStatus.Gepauzeerd,
            datum: DateTime.Today,
            start: new TimeSpan(9, 0, 0)
        ));
        await db.SaveChangesAsync();

        var c = new VeilingPublicController(db);
        var result = await c.GetActief();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

        Assert.True(dto.IsPauze);
        Assert.False(dto.IsGestart);
    }

    // HuidigProductId niet gevonden geeft HuidigProduct null, wachtrij nog gevuld
    [Fact]
    public async Task GetActief_HuidigProductNietGevonden_HuidigNull()
    {
        using var db = TestDb.Create();

        var a2 = TestData.Aanmelding(id: 2, soort: "B");
        var p2 = TestData.Product(id: 102, volgorde: 2, aanmelding: a2, actief: false);

        db.Veilingen.Add(TestData.Veiling(
            id: 1,
            status: VeilingStatus.Gestart,
            datum: DateTime.Today,
            start: new TimeSpan(9, 0, 0),
            huidigProductId: 999,
            producten: new() { p2 }
        ));
        await db.SaveChangesAsync();

        var c = new VeilingPublicController(db);
        var result = await c.GetActief();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

        Assert.Null(dto.HuidigProduct);
        Assert.Single(dto.Wachtrij);
    }

    // Volgende: geen geplande veilingen geeft null
    [Fact]
    public async Task GetVolgende_Geen_ReturnsNull()
    {
        using var db = TestDb.Create();
        var c = new VeilingPublicController(db);

        var res = await c.GetVolgende();

        var ok = Assert.IsType<OkObjectResult>(res.Result);
        Assert.Null(ok.Value);
    }

    // Volgende: kiest juiste veiling en telt producten
    [Fact]
    public async Task GetVolgende_Werkt_KiestEersteEnTelt()
    {
        using var db = TestDb.Create();
        var today = DateTime.Today;
        var now = DateTime.Now.TimeOfDay;

        db.Veilingen.AddRange(
            TestData.Veiling(1, VeilingStatus.Gepland, today, now + TimeSpan.FromMinutes(10)),
            TestData.Veiling(2, VeilingStatus.Gepland, today.AddDays(1), new TimeSpan(9, 0, 0))
        );

        db.VeilingProducten.AddRange(
            new Veilingklok.Core.Entities.VeilingProduct { Id = 11, VeilingId = 1, Volgorde = 1 },
            new Veilingklok.Core.Entities.VeilingProduct { Id = 12, VeilingId = 1, Volgorde = 2 }
        );

        await db.SaveChangesAsync();

        var c = new VeilingPublicController(db);
        var res = await c.GetVolgende();

        var ok = Assert.IsType<OkObjectResult>(res.Result);
        var dto = Assert.IsType<GeplandeVeilingListItemDto>(ok.Value);

        Assert.Equal(1, dto.Id);
        Assert.Equal(2, dto.AantalProducten);
    }
}
