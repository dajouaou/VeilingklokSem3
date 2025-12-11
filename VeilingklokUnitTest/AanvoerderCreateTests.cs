using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerderDashboard.Services;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Core.Enums;
using System;
using System.Threading.Tasks;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class AanvoerderCreateTests
    {
        [TestMethod]
        public async Task CreateAanmelding_GeldigeData_MaaktAanmelding()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });
            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);

            var dto = new AanmeldingCreateDto
            {
                Soort = "Roos",
                Potmaat = "10",
                Hoeveelheid = 5,
                MinimumPrijs = 2.5m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                Veildatum = DateTime.Today.AddDays(1)
            };

            var result = await service.CreateAanmeldingAsync(10, dto, "foto.jpg");

            Assert.IsNotNull(result);
            Assert.AreEqual("Roos", result.Soort);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateAanmelding_Zaterdag_GooitFout()
        {
            using var db = TestDbHelper.CreateInMemory();
            db.Aanvoerders.Add(new Aanvoerder { Id = 1, GebruikerId = 10 });
            db.SaveChanges();

            var service = new AanvoerderDashboardService(db);

            var dto = new AanmeldingCreateDto
            {
                Soort = "Test",
                Hoeveelheid = 5,
                MinimumPrijs = 3,
                KlokLocatie = KlokLocatie.Aalsmeer,
                Veildatum = new DateTime(2025, 1, 4) // zaterdag
            };

            await service.CreateAanmeldingAsync(10, dto, null);
        }
    }
}
