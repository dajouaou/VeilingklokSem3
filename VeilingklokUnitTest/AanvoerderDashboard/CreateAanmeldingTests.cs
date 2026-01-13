using System;
using System.Threading.Tasks;
using Veilingklok.Core.Enums;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Features.AanvoerderDashboard.Services;
using VeilingklokUnitTest.AanvoerderDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.AanvoerderDashboard
{
    // Tests voor het aanmaken van een aanmelding
    public class CreateAanmeldingTests
    {
        [Fact]
        public async Task Create_NoAanvoerderProfile_Throws()
        {
            // Maak een lege database zonder aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            var service = new AanvoerderDashboardService(db);

            // Geldige input, maar gebruiker heeft geen aanvoerderprofiel
            var dto = new AanmeldingCreateDto
            {
                Soort = "Roos",
                Potmaat = "12cm",
                Hoeveelheid = 10,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today
            };

            // Verwacht een fout omdat de gebruiker geen aanvoerder is
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateAanmeldingAsync(gebruikerId: 999, dto, fotoUrl: null));

            Assert.Contains("Geen aanvoerder-profiel", ex.Message);
        }

        [Fact]
        public async Task Create_WithoutPotmaatAndSteellengte_Throws()
        {
            // Database met geldige aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 1, aanvoerderId: 10);

            var service = new AanvoerderDashboardService(db);

            // Potmaat en steellengte ontbreken
            var dto = new AanmeldingCreateDto
            {
                Soort = "Tulp",
                Potmaat = null,
                Steellengte = null,
                Hoeveelheid = 5,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today
            };

            // Verwacht een validatiefout
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateAanmeldingAsync(1, dto, null));

            Assert.Contains("potmaat of steellengte", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Create_OnWeekendNotToday_Throws()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 2, aanvoerderId: 20);

            var service = new AanvoerderDashboardService(db);

            // Zoek een zaterdag die niet vandaag is
            var saturday = AanvoerderDashboardTestFactory.NextDayOfWeek(DateTime.Today.AddDays(1), DayOfWeek.Saturday);
            if (saturday == DateTime.Today) saturday = saturday.AddDays(7);

            var dto = new AanmeldingCreateDto
            {
                Soort = "WeekendTest",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = saturday
            };

            // Weekend is niet toegestaan
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateAanmeldingAsync(2, dto, null));

            Assert.Contains("Zaterdag en zondag", ex.Message);
        }

        [Fact]
        public async Task Create_OnFeestdag_Throws()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 3, aanvoerderId: 30);

            var service = new AanvoerderDashboardService(db);

            // Feestdag die in de service is vastgelegd
            var feestdag = new DateTime(2025, 12, 25);
            if (feestdag == DateTime.Today) feestdag = feestdag.AddDays(1);

            var dto = new AanmeldingCreateDto
            {
                Soort = "FeestdagTest",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = feestdag
            };

            // Op feestdagen mag geen aanmelding worden gemaakt
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateAanmeldingAsync(3, dto, null));

            Assert.Contains("feestdag", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Create_Valid_CreatesRowAndReturnsDto()
        {
            // Database met geldige aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 4, aanvoerderId: 40, naam: "Jan");

            var service = new AanvoerderDashboardService(db);

            // Geldige input
            var dto = new AanmeldingCreateDto
            {
                Soort = "Roos",
                Potmaat = "12cm",
                Steellengte = null,
                Hoeveelheid = 10,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today,
                Beschrijving = "Nieuwe aanmelding"
            };

            // Aanmelding wordt succesvol aangemaakt
            var result = await service.CreateAanmeldingAsync(4, dto, fotoUrl: "http://foto");

            // Controleer resultaat en database
            Assert.True(result.Id > 0);
            Assert.Equal("Roos", result.Soort);
            Assert.Single(db.Aanmeldingen);
        }
    }
}
