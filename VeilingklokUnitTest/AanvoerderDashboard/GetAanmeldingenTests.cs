using System;
using System.Linq;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.AanvoerderDashboard.Services;
using VeilingklokUnitTest.AanvoerderDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.AanvoerderDashboard
{
    // Tests voor het ophalen van aanmeldingen
    public class GetAanmeldingenTests
    {
        [Fact]
        public async Task GetAanmeldingen_NoAanvoerderProfile_Throws()
        {
            // Database zonder aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            var service = new AanvoerderDashboardService(db);

            // Verwacht een fout omdat de gebruiker geen aanvoerderprofiel heeft
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetAanmeldingenAsync(gebruikerId: 999, veildatum: null));
        }

        [Fact]
        public async Task GetAanmeldingen_EmptyResult_ReturnsEmptyList()
        {
            // Database met aanvoerder maar zonder aanmeldingen
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 30, aanvoerderId: 10);

            var service = new AanvoerderDashboardService(db);

            // Ophaalactie zonder filters
            var list = await service.GetAanmeldingenAsync(30, null);

            // Resultaat moet leeg zijn
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAanmeldingen_FiltersOnDate()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 31, aanvoerderId: 11, naam: "Piet");

            // Twee aanmeldingen op verschillende dagen
            db.Aanmeldingen.AddRange(
                new Aanmelding
                {
                    Id = 1,
                    AanvoerderId = 11,
                    Soort = "A",
                    Potmaat = "10cm",
                    Hoeveelheid = 1,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = new DateTime(2025, 1, 10)
                },
                new Aanmelding
                {
                    Id = 2,
                    AanvoerderId = 11,
                    Soort = "B",
                    Potmaat = "10cm",
                    Hoeveelheid = 1,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = new DateTime(2025, 1, 11)
                }
            );
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Filter op één specifieke datum
            var list = await service.GetAanmeldingenAsync(31, new DateTime(2025, 1, 10));

            // Alleen de aanmelding van die dag mag terugkomen
            Assert.Single(list);
            Assert.Equal(1, list[0].Id);
        }

        [Fact]
        public async Task GetAanmeldingen_MapsVerkochtKoperPrijsOpbrengstEnAanvoerderNaam()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 32, aanvoerderId: 12, naam: "Klaas");

            // Aanmelding die verkocht is en gekoppeld aan een veilingproduct
            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 10,
                AanvoerderId = 12,
                Soort = "Roos",
                Potmaat = "12cm",
                Hoeveelheid = 10,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today,
                VeilingProduct = new VeilingProduct
                {
                    Id = 99,
                    IsVerkocht = true,
                    HuidigePrijs = 2m,
                    Koper = new Koper
                    {
                        Id = 1,
                        Naam = "Koper X"
                    }
                }
            });
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Aanmeldingen ophalen zonder filter
            var list = await service.GetAanmeldingenAsync(32, null);

            var item = Assert.Single(list);

            // Controleren of alle velden correct gemapt zijn
            Assert.True(item.IsVerkocht);
            Assert.Equal("Koper X", item.KoperNaam);
            Assert.Equal(2m, item.VerkoopPrijs);
            Assert.Equal(20m, item.TotaleOpbrengst);
            Assert.Equal("Klaas", item.AanvoerderNaam);
        }
    }
}
