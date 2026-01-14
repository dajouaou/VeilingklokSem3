using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using VeilingklokUnitTest.Helpers;
using Xunit;

namespace VeilingklokUnitTest.Veilingmeester;

public class VeilingPlanningControllerTests
{
    // Veildagen faalt niet en geeft lijst
    [Fact]
    public async Task GetVeildagen_Werkt()
    {
        using var db = TestDb.Create();
        db.Aanmeldingen.Add(TestData.Aanmelding(id: 1, leverDatum: DateTime.Today.AddDays(2)));
        await db.SaveChangesAsync();

        var c = new VeilingPlanningController(db);
        var res = await c.GetVeildagen();

        var ok = Assert.IsType<OkObjectResult>(res);
        var list = Assert.IsAssignableFrom<IEnumerable<string>>(ok.Value);
        Assert.NotEmpty(list);
    }

    // Aanmeldingen: ongeldige datum faalt
    [Fact]
    public async Task GetAanmeldingen_OngeldigeDatum_Faalt()
    {
        using var db = TestDb.Create();
        var c = new VeilingPlanningController(db);

        var res = await c.GetAanmeldingen("xx");

        Assert.IsType<BadRequestObjectResult>(res);
    }

    // Aanmeldingen: juiste datum werkt
    [Fact]
    public async Task GetAanmeldingen_GeldigeDatum_Werkt()
    {
        using var db = TestDb.Create();
        var lever = new DateTime(2026, 1, 10);

        db.Aanmeldingen.Add(TestData.Aanmelding(id: 1, leverDatum: lever, soort: "A"));
        await db.SaveChangesAsync();

        var c = new VeilingPlanningController(db);
        var res = await c.GetAanmeldingen("2026-01-10");

        var ok = Assert.IsType<OkObjectResult>(res);
        var items = Assert.IsType<List<VeilingPlanningAanmeldingDto>>(ok.Value);
        Assert.Single(items);
    }

    // Plan veiling: verkeerde input faalt
    [Fact]
    public async Task PlanVeiling_VerkeerdeInput_Faalt()
    {
        using var db = TestDb.Create();
        var c = new VeilingPlanningController(db);

        Assert.IsType<BadRequestObjectResult>(await c.PlanVeiling(new PlanVeilingRequestDto { Leverdatum = "x", Veildatum = "2026-01-01", StartTijd = "09:00" }));
        Assert.IsType<BadRequestObjectResult>(await c.PlanVeiling(new PlanVeilingRequestDto { Leverdatum = "2026-01-01", Veildatum = "x", StartTijd = "09:00" }));
        Assert.IsType<BadRequestObjectResult>(await c.PlanVeiling(new PlanVeilingRequestDto { Leverdatum = "2026-01-01", Veildatum = "2026-01-02", StartTijd = "xx" }));
    }

    // Plan veiling: dubbel plannen faalt
    [Fact]
    public async Task PlanVeiling_Dubbel_Faalt()
    {
        using var db = TestDb.Create();
        var c = new VeilingPlanningController(db);

        var lever = DateTime.Today.AddDays(7);
        db.Aanmeldingen.AddRange(
            TestData.Aanmelding(id: 1, leverDatum: lever),
            TestData.Aanmelding(id: 2, leverDatum: lever)
        );
        await db.SaveChangesAsync();

        var start = DateTime.Now.AddMinutes(10);

        var dto = new PlanVeilingRequestDto
        {
            Leverdatum = lever.ToString("yyyy-MM-dd"),
            Veildatum = start.Date.ToString("yyyy-MM-dd"),
            StartTijd = start.TimeOfDay.ToString(@"hh\:mm"),
            AanmeldingIds = new() { 1, 2 }
        };

        Assert.IsType<OkObjectResult>(await c.PlanVeiling(dto));
        Assert.IsType<BadRequestObjectResult>(await c.PlanVeiling(dto));
    }

    // GetVolgende: geen veiling geeft null
    [Fact]
    public async Task GetVolgende_Geen_ReturnsNull()
    {
        using var db = TestDb.Create();
        var c = new VeilingPlanningController(db);

        var res = await c.GetVolgende();

        var ok = Assert.IsType<OkObjectResult>(res);
        Assert.Null(ok.Value);
    }

    // GetGeplande: telt producten
    [Fact]
    public async Task GetGeplande_Werkt_Telt()
    {
        using var db = TestDb.Create();
        var c = new VeilingPlanningController(db);

        var today = DateTime.Today;
        var start = DateTime.Now.AddMinutes(10).TimeOfDay;

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gepland, today, start));
        db.VeilingProducten.AddRange(
            new Veilingklok.Core.Entities.VeilingProduct { Id = 1, VeilingId = 1, Volgorde = 1 },
            new Veilingklok.Core.Entities.VeilingProduct { Id = 2, VeilingId = 1, Volgorde = 2 }
        );
        await db.SaveChangesAsync();

        var res = await c.GetGeplande();
        var ok = Assert.IsType<OkObjectResult>(res);
        var list = Assert.IsType<List<Veilingklok.Features.VeilingmeesterDashboard.Dtos.GeplandeVeilingListItemDto>>(ok.Value);

        Assert.Single(list);
        Assert.Equal(2, list[0].AantalProducten);
    }
}
