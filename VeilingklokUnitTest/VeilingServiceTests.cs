using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.Veiling.Services;
using Veilingklok.Infrastructure.Database;

[TestClass]
public class VeilingServiceTests
{
    private MyContext CreateDb()
    {
        return new MyContext(
            new DbContextOptionsBuilder<MyContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options
        );
    }

    [TestMethod]
    public async Task StartVeiling_CreëertVeilingEnEersteProductActief()
    {
        using var db = CreateDb();

        db.Aanmeldingen.Add(new Aanmelding { Id = 1, Soort = "Roos", MinimumPrijs = 2, Veildatum = DateTime.Today });
        db.SaveChanges();

        var service = new VeilingService(db);

        var result = await service.StartVeilingAsync(DateTime.Today);

        Assert.IsTrue(result.IsGestart);
        Assert.IsNotNull(result.HuidigProduct);
        Assert.AreEqual("Roos", result.HuidigProduct.Soort);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task StartVeiling_GeenAanmeldingen_GooiExceptie()
    {
        using var db = CreateDb();
        var service = new VeilingService(db);

        await service.StartVeilingAsync(DateTime.Today);
    }

    [TestMethod]
    public async Task Pause_ZetStatusOpGepauzeerd()
    {
        using var db = CreateDb();
        db.Veilingen.Add(new Veiling { Id = 1, Status = VeilingStatus.Gestart });
        db.SaveChanges();

        var service = new VeilingService(db);
        await service.PauseAsync(1);

        Assert.AreEqual(VeilingStatus.Gepauzeerd, db.Veilingen.Find(1)!.Status);
    }

    [TestMethod]
    public async Task Resume_ZetStatusOpGestart()
    {
        using var db = CreateDb();
        db.Veilingen.Add(new Veiling { Id = 1, Status = VeilingStatus.Gepauzeerd });
        db.SaveChanges();

        var service = new VeilingService(db);
        await service.ResumeAsync(1);

        Assert.AreEqual(VeilingStatus.Gestart, db.Veilingen.Find(1)!.Status);
    }

    [TestMethod]
    public async Task Stop_ZetStatusOpAfgesloten()
    {
        using var db = CreateDb();
        db.Veilingen.Add(new Veiling { Id = 1, Status = VeilingStatus.Gestart });
        db.SaveChanges();

        var service = new VeilingService(db);
        await service.StopAsync(1);

        Assert.AreEqual(VeilingStatus.Afgesloten, db.Veilingen.Find(1)!.Status);
    }

    [TestMethod]
    public async Task PlaatsBod_VerkooptProductEnActiveertVolgende()
    {
        using var db = CreateDb();

        // Veiling + 2 producten
        db.Veilingen.Add(new Veiling { Id = 1, Status = VeilingStatus.Gestart });
        db.Aanmeldingen.Add(new Aanmelding { Id = 1, Soort = "Roos", MinimumPrijs = 2, Hoeveelheid = 5 });
        db.Aanmeldingen.Add(new Aanmelding { Id = 2, Soort = "Tulp", MinimumPrijs = 3, Hoeveelheid = 7 });
        db.SaveChanges();

        db.VeilingProducten.Add(new VeilingProduct { Id = 10, VeilingId = 1, AanmeldingId = 1, IsActief = true, Volgorde = 1 });
        db.VeilingProducten.Add(new VeilingProduct { Id = 11, VeilingId = 1, AanmeldingId = 2, IsActief = false, Volgorde = 2 });
        db.SaveChanges();

        var service = new VeilingService(db);

        await service.PlaatsBodAsync(1, new BodPlaatsenDto { VeilingProductId = 10, Prijs = 5 }, koperId: 5);

        var actief = db.VeilingProducten.Single(p => p.IsActief);
        Assert.AreEqual(11, actief.Id);

        var verkocht = db.VeilingProducten.Single(p => p.Id == 10);
        Assert.IsTrue(verkocht.IsVerkocht);
    }
}

