using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerderDashboard.Services;
using System.Threading.Tasks;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class AanvoerderDeleteTests
    {
        [TestMethod]
        public async Task DeleteAanmelding_VerwijdertItem()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });
            db.Aanmeldingen.Add(new Aanmelding { Id = 3, AanvoerderId = 1 });
            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);

            await service.DeleteAanmeldingAsync(10, 3);

            Assert.AreEqual(0, db.Aanmeldingen.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteAanmelding_NietGevonden_GooitFout()
        {
            using var db = TestDbHelper.CreateInMemory();
            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });
            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);
            await service.DeleteAanmeldingAsync(10, 999);
        }
    }
}
