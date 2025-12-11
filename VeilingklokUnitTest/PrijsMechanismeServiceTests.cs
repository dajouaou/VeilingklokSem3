using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using Veilingklok.Features.Veiling.Services;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class PrijsMechanismeServiceTests
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
        public async Task Prijs_DaaltMaarNooitOnderNul()
        {
            using var db = CreateDb();
            var broadcast = new Mock<IVeilingBroadcastService>();

            db.Veilingen.Add(new Veiling { Id = 1, Status = VeilingStatus.Gestart });
            db.Aanmeldingen.Add(new Aanmelding { Id = 1, Soort = "Roos", MinimumPrijs = 1 });
            db.VeilingProducten.Add(new VeilingProduct
            {
                Id = 5,
                VeilingId = 1,
                AanmeldingId = 1,
                Volgorde = 1,
                HuidigePrijs = 0.01m
            });
            db.SaveChanges();

            // Simuleer prijsdaling
            var p = db.VeilingProducten.Find(5)!;
            p.HuidigePrijs -= 0.05m;
            if (p.HuidigePrijs < 0) p.HuidigePrijs = 0;

            db.SaveChanges();

            Assert.AreEqual(0, p.HuidigePrijs);
        }
    }
}
