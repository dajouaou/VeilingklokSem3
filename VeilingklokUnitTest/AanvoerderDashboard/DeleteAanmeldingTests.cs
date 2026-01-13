using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.AanvoerderDashboard.Services;
using Veilingklok.Infrastructure.Database;
using VeilingklokUnitTest.AanvoerderDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.AanvoerderDashboard
{
    // Tests voor het verwijderen van een aanmelding
    public class DeleteAanmeldingTests
    {
        [Fact]
        public async Task Delete_NotFound_Throws()
        {
            // Maak database met een geldige aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 20, aanvoerderId: 1);

            var service = new AanvoerderDashboardService(db);

            // Probeer een niet-bestaande aanmelding te verwijderen
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.DeleteAanmeldingAsync(20, 999));

            // Verwacht foutmelding omdat de aanmelding niet bestaat
            Assert.Contains("niet gevonden", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Delete_WrongOwner_ThrowsNotFound()
        {
            // Twee verschillende aanvoerders in de database
            using var db = AanvoerderDashboardTestFactory.CreateDb();

            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 21, aanvoerderId: 1, naam: "A");
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 22, aanvoerderId: 2, naam: "B");

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

            // Aanvoerder B probeert deze aanmelding te verwijderen
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.DeleteAanmeldingAsync(22, 100));

            // Dit mag niet en moet een fout geven
            Assert.Contains("niet gevonden", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Delete_LinkedToVeilingProduct_Throws()
        {
            // Database met aanvoerder
            using var db = AanvoerderDashboardTestFactory.CreateDb();
            await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(db, gebruikerId: 23, aanvoerderId: 3);

            // Aanmelding is al gekoppeld aan een veiling
            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 200,
                AanvoerderId = 3,
                Soort = "X",
                Potmaat = "10cm",
                Hoeveelheid = 1,
                MinimumPrijs = 1m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = DateTime.Today,
                VeilingProductId = 7
            });
            await db.SaveChangesAsync();

            var service = new AanvoerderDashboardService(db);

            // Verwijderen mag niet als de aanmelding al in een veiling zit
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.DeleteAanmeldingAsync(23, 200));

            Assert.Contains("Kan niet verwijderen", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Delete_Valid_RemovesRow()
        {
            // We gebruiken dezelfde InMemory database met twee contexts
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            int id;

            // Context 1: data aanmaken
            using (var dbSeed = new MyContext(options))
            {
                await AanvoerderDashboardTestFactory.SeedAanvoerderAsync(dbSeed, gebruikerId: 24, aanvoerderId: 4);

                var aanmelding = new Aanmelding
                {
                    AanvoerderId = 4,
                    Soort = "X",
                    Potmaat = "10cm",
                    Hoeveelheid = 1,
                    MinimumPrijs = 1m,
                    KlokLocatie = KlokLocatie.Aalsmeer,
                    LeverDatum = DateTime.Today
                };

                dbSeed.Aanmeldingen.Add(aanmelding);
                await dbSeed.SaveChangesAsync();

                // Id opslaan om later te kunnen verwijderen
                id = aanmelding.Id;
            }

            // Context 2: verwijderen zonder tracking-problemen
            using (var dbDelete = new MyContext(options))
            {
                var service = new AanvoerderDashboardService(dbDelete);

                await service.DeleteAanmeldingAsync(24, id);

                // Controleren dat de aanmelding echt weg is
                Assert.Null(await dbDelete.Aanmeldingen.FindAsync(id));
            }
        }
    }
}
