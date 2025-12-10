using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerderDashboard.Services;
using System.Threading.Tasks;
using System.Linq;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class AanvoerderQueryTests
    {
        [TestMethod]
        public async Task GetAanmeldingen_PlaatsVerkochteEnNietVerkochteCorrect()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });

            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 1,
                AanvoerderId = 1,
                Hoeveelheid = 10,
                VeilingProduct = new VeilingProduct
                {
                    IsVerkocht = true,
                    HuidigePrijs = 5,
                    Koper = new Koper { Naam = "Piet" }
                }
            });

            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 2,
                AanvoerderId = 1,
                Hoeveelheid = 4
            });

            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);

            var list = await service.GetAanmeldingenAsync(10, null);

            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Single(x => x.Id == 1).IsVerkocht);
            Assert.IsFalse(list.Single(x => x.Id == 2).IsVerkocht);
        }

        [TestMethod]
        public async Task GetStats_BerekeningKlopt()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });

            db.Aanmeldingen.Add(new Aanmelding
            {
                Hoeveelheid = 10,
                AanvoerderId = 1,
                VeilingProduct = new VeilingProduct { IsVerkocht = true, HuidigePrijs = 5 }
            });

            db.Aanmeldingen.Add(new Aanmelding
            {
                Hoeveelheid = 4,
                AanvoerderId = 1
            });

            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);
            var stats = await service.GetStatsAsync(10, null);

            Assert.AreEqual(2, stats.TotaalAantalAanmeldingen);
            Assert.AreEqual(1, stats.AantalVerkocht);
            Assert.AreEqual(50m, stats.TotaleOpbrengst);
        }
    }
}
