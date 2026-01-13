using System;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Features.AanvoerderDashboard.Services;
using VeilingklokUnitTest.AanvoerderDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.AanvoerderDashboard
{
    // Tests voor het updaten van een aanmelding
    public class UpdateAanmeldingTests
    {
        [Fact]
        public async Task Update_NotFound_Throws()
        {
            // Database met aanvoerder, maar geen aanmelding met id 999
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 10, aanvoerderId: 1);

            var service = new AanvoerderDashboardService(db);

            // Update data die we willen meegeven
            var dto = new AanmeldingUpdateDto
            {
                Soort = "Update",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today
            };

            // Verwacht fout omdat het id niet bestaat
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateAanmeldingAsync(10, 999, dto, null));

            Assert.Contains("niet gevonden", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Update_WrongOwner_ThrowsNotFound()
        {
            // Twee aanvoerders: A en B
            using var db = AanvoerderDashboardTestFactory.CreateDb();

            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 11, aanvoerderId: 1, naam: "A");
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 12, aanvoerderId: 2, naam: "B");

            // Aanmelding hoort bij aanvoerder A
            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 100,
                AanvoerderId = 1,
                Soort = "X",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today
            });
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Aanvoerder B probeert de aanmelding van A te updaten
            var dto = new AanmeldingUpdateDto
            {
                Soort = "Update",
                Potmaat = "12cm",
                Hoeveelheid = 2,
                MinimumPrijs = 2m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today
            };

            // Dit mag niet, dus verwacht NotFound gedrag (service geeft ArgumentException)
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateAanmeldingAsync(12, 100, dto, null));

            Assert.Contains("niet gevonden", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Update_Valid_UpdatesFields()
        {
            // Database met aanvoerder en een bestaande aanmelding
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 13, aanvoerderId: 3);

            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 200,
                AanvoerderId = 3,
                Soort = "Oud",
                Potmaat = "8cm",
                Steellengte = null,
                Hoeveelheid = 5,
                MinimumPrijs = 0.50m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today,
                FotoUrl = "KEEP",          // moet blijven als we geen nieuwe foto meegeven
                Beschrijving = "old"
            });
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Nieuwe waarden die de oude waarden moeten overschrijven
            var dto = new AanmeldingUpdateDto
            {
                Soort = "Nieuw",
                Potmaat = "12cm",
                Steellengte = "40",
                Hoeveelheid = 10,
                MinimumPrijs = 1.25m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today.AddDays(1),
                Beschrijving = "new"
            };

            // Update uitvoeren
            await service.UpdateAanmeldingAsync(13, 200, dto, fotoUrl: null);

            // Check in de database of alles echt is aangepast
            var saved = await db.Aanmeldingen.FindAsync(200);
            Assert.NotNull(saved);

            Assert.Equal("Nieuw", saved!.Soort);
            Assert.Equal("12cm", saved.Potmaat);
            Assert.Equal("40", saved.Steellengte);
            Assert.Equal(10, saved.Hoeveelheid);
            Assert.Equal(1.25m, saved.MinimumPrijs);
            Assert.Equal("KEEP", saved.FotoUrl); // foto blijft hetzelfde omdat fotoUrl null is
            Assert.Equal(DateTime.Today.AddDays(1).Date, saved.LeverDatum);
            Assert.Equal("new", saved.Beschrijving);
        }

        [Fact]
        public async Task Update_FotoUrlProvided_OverwritesFoto()
        {
            // Database met aanvoerder en een aanmelding met bestaande foto
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 14, aanvoerderId: 4);

            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 300,
                AanvoerderId = 4,
                Soort = "A",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today,
                FotoUrl = "OLD"
            });
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Update zonder andere wijzigingen, maar wel met nieuwe fotoUrl
            var dto = new AanmeldingUpdateDto
            {
                Soort = "A",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today
            };

            // Foto moet nu vervangen worden
            await service.UpdateAanmeldingAsync(14, 300, dto, fotoUrl: "NEW");

            var saved = await db.Aanmeldingen.FindAsync(300);
            Assert.NotNull(saved);
            Assert.Equal("NEW", saved!.FotoUrl);
        }
    }
}
