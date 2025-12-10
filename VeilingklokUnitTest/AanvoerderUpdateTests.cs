using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Features.AanvoerderDashboard.Services;
using Veilingklok.Core.Enums;
using System;
using System.Threading.Tasks;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class AanvoerderUpdateTests
    {
        [TestMethod]
        public async Task UpdateAanmelding_WerktCorrect()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });
            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 5,
                AanvoerderId = 1,
                Soort = "Oud"
            });
            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);

            var dto = new AanmeldingUpdateDto
            {
                Soort = "Nieuw",
                Hoeveelheid = 7,
                MinimumPrijs = 1.5m,
                KlokLocatie = KlokLocatie.Naaldwijk,
                Veildatum = DateTime.Today
            };

            var result = await service.UpdateAanmeldingAsync(10, 5, dto, null);

            Assert.AreEqual("Nieuw", result.Soort);
            Assert.AreEqual(7, result.Hoeveelheid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateAanmelding_NietGevonden_GooitFout()
        {
            using var db = TestDbHelper.CreateInMemory();
            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });
            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);

            var dto = new AanmeldingUpdateDto
            {
                Soort = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 1,
                KlokLocatie = KlokLocatie.Aalsmeer,
                Veildatum = DateTime.Today
            };

            await service.UpdateAanmeldingAsync(10, 999, dto, null);
        }
    }
}
