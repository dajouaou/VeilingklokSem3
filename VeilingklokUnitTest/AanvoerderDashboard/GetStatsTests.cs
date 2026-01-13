using System;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.AanvoerderDashboard.Services;
using VeilingklokUnitTest.AanvoerderDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.AanvoerderDashboard
{
    // Tests voor de statistieken (totaal, verkocht, opbrengst)
    public class GetStatsTests
    {
        [Fact]
        public async Task Stats_NoAanvoerderProfile_Throws()
        {
            // Database zonder aanvoerderprofiel
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            var service = new AanvoerderDashboardService(db);

            // Verwacht fout omdat deze gebruiker geen aanvoerder is
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetStatsAsync(gebruikerId: 999, veildatum: null));
        }

        [Fact]
        public async Task Stats_EmptyResult_ReturnsZeros()
        {
            // Database met aanvoerder maar zonder aanmeldingen
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 40, aanvoerderId: 1);

            var service = new AanvoerderDashboardService(db);

            // Statistieken ophalen terwijl er geen data is
            var stats = await service.GetStatsAsync(40, null);

            // Alles moet dan gewoon 0 zijn
            Assert.Equal(0, stats.TotaalAantalAanmeldingen);
            Assert.Equal(0, stats.AantalVerkocht);
            Assert.Equal(0m, stats.TotaleOpbrengst);
        }

        [Fact]
        public async Task Stats_ComputesTotalsAndRevenue()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 41, aanvoerderId: 2);

            // 2 aanmeldingen: 1 verkocht en 1 niet verkocht
            db.Aanmeldingen.AddRange(
                new Aanmelding
                {
                    Id = 1,
                    AanvoerderId = 2,
                    Soort = "A",
                    Potmaat = "10cm",
                    Hoeveelheid = 10,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = DateTime.Today,
                    VeilingProduct = new VeilingProduct { Id = 1, IsVerkocht = true, HuidigePrijs = 1.50m }
                },
                new Aanmelding
                {
                    Id = 2,
                    AanvoerderId = 2,
                    Soort = "B",
                    Potmaat = "10cm",
                    Hoeveelheid = 5,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = DateTime.Today,
                    VeilingProduct = new VeilingProduct { Id = 2, IsVerkocht = false, HuidigePrijs = 9.99m }
                }
            );
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Statistieken ophalen voor vandaag
            var stats = await service.GetStatsAsync(41, DateTime.Today);

            // Totaal is 2, verkocht is 1, opbrengst = 10 * 1.50 = 15
            Assert.Equal(2, stats.TotaalAantalAanmeldingen);
            Assert.Equal(1, stats.AantalVerkocht);
            Assert.Equal(15m, stats.TotaleOpbrengst);
        }

        [Fact]
        public async Task Stats_FiltersOnDate()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 42, aanvoerderId: 3);

            // 2 verkochte aanmeldingen op verschillende dagen
            db.Aanmeldingen.AddRange(
                new Aanmelding
                {
                    Id = 1,
                    AanvoerderId = 3,
                    Soort = "A",
                    Potmaat = "10cm",
                    Hoeveelheid = 10,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = new DateTime(2025, 2, 1),
                    VeilingProduct = new VeilingProduct { Id = 1, IsVerkocht = true, HuidigePrijs = 2m }
                },
                new Aanmelding
                {
                    Id = 2,
                    AanvoerderId = 3,
                    Soort = "B",
                    Potmaat = "10cm",
                    Hoeveelheid = 10,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = new DateTime(2025, 2, 2),
                    VeilingProduct = new VeilingProduct { Id = 2, IsVerkocht = true, HuidigePrijs = 3m }
                }
            );
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Filter op 2025-02-01, dus alleen de eerste telt mee
            var stats = await service.GetStatsAsync(42, new DateTime(2025, 2, 1));

            // Totaal 1, verkocht 1, opbrengst = 10 * 2 = 20
            Assert.Equal(1, stats.TotaalAantalAanmeldingen);
            Assert.Equal(1, stats.AantalVerkocht);
            Assert.Equal(20m, stats.TotaleOpbrengst);
        }
    }
}
