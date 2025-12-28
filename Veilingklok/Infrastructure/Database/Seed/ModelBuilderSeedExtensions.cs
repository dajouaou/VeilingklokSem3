using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Infrastructure.Database;

public static class ModelBuilderSeedExtensions
{
    public static void SeedDemoData(this ModelBuilder modelBuilder)
    {
        var createdUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var img1 = "/AIimg/ai_self_made_pic_1.jpg.png";
        var img2 = "/AIimg/ai_self_made_pic_2.jpg.png";
        var img3 = "/AIimg/ai_self_made_pic_3.jpg.png";
        var img4 = "/AIimg/ai_self_made_pic_4.jpg.png";

        var leverdag1 = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Unspecified);
        var leverdag2 = new DateTime(2025, 1, 11, 0, 0, 0, DateTimeKind.Unspecified);

        var veildatum1 = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Unspecified);
        var veildatum2 = new DateTime(2025, 1, 13, 0, 0, 0, DateTimeKind.Unspecified);

        modelBuilder.Entity<Gebruiker>().HasData(
            new Gebruiker { Id = 1, Email = "koper1@email.com", PasswordHash = "DEMO_HASH", Voornaam = "Koper", Achternaam = "1", Rol = UserRole.Koper, CreatedAtUtc = createdUtc },
            new Gebruiker { Id = 2, Email = "koper2@email.com", PasswordHash = "DEMO_HASH", Voornaam = "Koper", Achternaam = "2", Rol = UserRole.Koper, CreatedAtUtc = createdUtc },

            new Gebruiker { Id = 3, Email = "vm1@email.com", PasswordHash = "DEMO_HASH", Voornaam = "VM", Achternaam = "1", Rol = UserRole.VM, CreatedAtUtc = createdUtc },
            new Gebruiker { Id = 4, Email = "vm2@email.com", PasswordHash = "DEMO_HASH", Voornaam = "VM", Achternaam = "2", Rol = UserRole.VM, CreatedAtUtc = createdUtc },

            new Gebruiker { Id = 5, Email = "aanvoerder1@email.com", PasswordHash = "DEMO_HASH", Voornaam = "Aanvoerder", Achternaam = "1", Rol = UserRole.Aanvoerder, CreatedAtUtc = createdUtc },
            new Gebruiker { Id = 6, Email = "aanvoerder2@email.com", PasswordHash = "DEMO_HASH", Voornaam = "Aanvoerder", Achternaam = "2", Rol = UserRole.Aanvoerder, CreatedAtUtc = createdUtc }
        );

        modelBuilder.Entity<Koper>().HasData(
            new Koper { Id = 1, GebruikerId = 1, Naam = "Koper 1 BV" },
            new Koper { Id = 2, GebruikerId = 2, Naam = "Koper 2 BV" }
        );

        modelBuilder.Entity<VM>().HasData(
            new VM { Id = 1, GebruikerId = 3, Naam = "Veilingmeester 1" },
            new VM { Id = 2, GebruikerId = 4, Naam = "Veilingmeester 2" }
        );

        modelBuilder.Entity<Aanvoerder>().HasData(
            new Aanvoerder { Id = 1, GebruikerId = 5, Naam = "Aanvoerder 1" },
            new Aanvoerder { Id = 2, GebruikerId = 6, Naam = "Aanvoerder 2" }
        );

        modelBuilder.Entity<Veildag>().HasData(
            new Veildag { Id = 1, Datum = leverdag1 },
            new Veildag { Id = 2, Datum = leverdag2 }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                AanvoerderId = 1,
                Naam = "Product A1-1",
                Categorie = "Bloemen",
                Beschrijving = "Demo product van Aanvoerder 1",
                FotoUrl = img1,
                Soort = "Roos",
                PotmaatOfSteellengte = "60cm",
                HoeveelheidStuks = 100,
                MinimumPrijs = 1.50m,
                KlokLocatie = "Naaldwijk",
                VeilDatum = veildatum1
            },
            new Product
            {
                Id = 2,
                AanvoerderId = 1,
                Naam = "Product A1-2",
                Categorie = "Bloemen",
                Beschrijving = "Demo product van Aanvoerder 1",
                FotoUrl = img2,
                Soort = "Tulp",
                PotmaatOfSteellengte = "40cm",
                HoeveelheidStuks = 80,
                MinimumPrijs = 1.20m,
                KlokLocatie = "Naaldwijk",
                VeilDatum = veildatum1
            },
            new Product
            {
                Id = 3,
                AanvoerderId = 2,
                Naam = "Product A2-1",
                Categorie = "Planten",
                Beschrijving = "Demo product van Aanvoerder 2",
                FotoUrl = img3,
                Soort = "Orchidee",
                PotmaatOfSteellengte = "12cm",
                HoeveelheidStuks = 60,
                MinimumPrijs = 2.00m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = veildatum2
            },
            new Product
            {
                Id = 4,
                AanvoerderId = 2,
                Naam = "Product A2-2",
                Categorie = "Planten",
                Beschrijving = "Demo product van Aanvoerder 2",
                FotoUrl = img4,
                Soort = "Lelie",
                PotmaatOfSteellengte = "70cm",
                HoeveelheidStuks = 50,
                MinimumPrijs = 2.50m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = veildatum2
            }
        );

        modelBuilder.Entity<Aanmelding>().HasData(
            new Aanmelding
            {
                Id = 1,
                AanvoerderId = 1,
                Soort = "Roos",
                Potmaat = null,
                Steellengte = "60cm",
                Hoeveelheid = 100,
                MinimumPrijs = 1.50m,
                KlokLocatie = KlokLocatie.Naaldwijk,
                LeverDatum = leverdag1,
                FotoUrl = img1,
                VeilingProductId = null,
                Beschrijving = "Aanmelding A1-1"
            },
            new Aanmelding
            {
                Id = 2,
                AanvoerderId = 1,
                Soort = "Tulp",
                Potmaat = null,
                Steellengte = "40cm",
                Hoeveelheid = 80,
                MinimumPrijs = 1.20m,
                KlokLocatie = KlokLocatie.Naaldwijk,
                LeverDatum = leverdag1,
                FotoUrl = img2,
                VeilingProductId = null,
                Beschrijving = "Aanmelding A1-2"
            },
            new Aanmelding
            {
                Id = 3,
                AanvoerderId = 2,
                Soort = "Orchidee",
                Potmaat = "12cm",
                Steellengte = null,
                Hoeveelheid = 60,
                MinimumPrijs = 2.00m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = leverdag2,
                FotoUrl = img3,
                VeilingProductId = null,
                Beschrijving = "Aanmelding A2-1"
            },
            new Aanmelding
            {
                Id = 4,
                AanvoerderId = 2,
                Soort = "Lelie",
                Potmaat = null,
                Steellengte = "70cm",
                Hoeveelheid = 50,
                MinimumPrijs = 2.50m,
                KlokLocatie = KlokLocatie.Aalsmeer,
                LeverDatum = leverdag2,
                FotoUrl = img4,
                VeilingProductId = null,
                Beschrijving = "Aanmelding A2-2"
            }
        );

        modelBuilder.Entity<Veiling>().HasData(
            new Veiling
            {
                Id = 1,
                Naam = "Veiling 1",
                Locatie = KlokLocatie.Naaldwijk,
                Datum = veildatum1,
                StartTijd = new TimeSpan(9, 0, 0),
                Status = VeilingStatus.Scheduled,
                VMId = 1,
                CurrentVeilingProductId = null,
                StartTijdUtc = null,
                EindTijdUtc = null
            },
            new Veiling
            {
                Id = 2,
                Naam = "Veiling 2",
                Locatie = KlokLocatie.Aalsmeer,
                Datum = veildatum2,
                StartTijd = new TimeSpan(10, 0, 0),
                Status = VeilingStatus.Scheduled,
                VMId = 2,
                CurrentVeilingProductId = null,
                StartTijdUtc = null,
                EindTijdUtc = null
            }
        );

        modelBuilder.Entity<VeilingProduct>().HasData(
            new
            {
                Id = 1,
                VeilingId = 1,
                AanmeldingId = 1,
                ProductId = (int?)1,
                AanvoerderId = (int?)1,
                Status = VeilingProductStatus.Queued,
                StartPrijs = 3.00m,
                HuidigePrijs = 3.00m,
                MinimumPrijs = 1.50m,
                DurationSeconds = 20,
                Hoeveelheid = 100,
                Volgorde = 1,
                ActivatedAtUtc = (DateTime?)null,
                ClosedAtUtc = (DateTime?)null,
                KoperId = (int?)null
            },
            new
            {
                Id = 2,
                VeilingId = 1,
                AanmeldingId = 2,
                ProductId = (int?)2,
                AanvoerderId = (int?)1,
                Status = VeilingProductStatus.Sold,
                StartPrijs = 2.50m,
                HuidigePrijs = 2.10m,
                MinimumPrijs = 1.20m,
                DurationSeconds = 20,
                Hoeveelheid = 80,
                Volgorde = 2,
                ActivatedAtUtc = (DateTime?)new DateTime(2025, 1, 12, 9, 5, 0, DateTimeKind.Utc),
                ClosedAtUtc = (DateTime?)new DateTime(2025, 1, 12, 9, 5, 12, DateTimeKind.Utc),
                KoperId = (int?)1
            },
            new
            {
                Id = 3,
                VeilingId = 2,
                AanmeldingId = 3,
                ProductId = (int?)3,
                AanvoerderId = (int?)2,
                Status = VeilingProductStatus.Queued,
                StartPrijs = 4.00m,
                HuidigePrijs = 4.00m,
                MinimumPrijs = 2.00m,
                DurationSeconds = 20,
                Hoeveelheid = 60,
                Volgorde = 1,
                ActivatedAtUtc = (DateTime?)null,
                ClosedAtUtc = (DateTime?)null,
                KoperId = (int?)null
            },
            new
            {
                Id = 4,
                VeilingId = 2,
                AanmeldingId = 4,
                ProductId = (int?)4,
                AanvoerderId = (int?)2,
                Status = VeilingProductStatus.Sold,
                StartPrijs = 5.00m,
                HuidigePrijs = 3.80m,
                MinimumPrijs = 2.50m,
                DurationSeconds = 20,
                Hoeveelheid = 50,
                Volgorde = 2,
                ActivatedAtUtc = (DateTime?)new DateTime(2025, 1, 13, 10, 2, 0, DateTimeKind.Utc),
                ClosedAtUtc = (DateTime?)new DateTime(2025, 1, 13, 10, 2, 9, DateTimeKind.Utc),
                KoperId = (int?)2
            }
        );

        modelBuilder.Entity<Bid>().HasData(
            new Bid
            {
                Id = 1,
                VeilingId = 1,
                VeilingProductId = 2,
                KoperId = 1,
                Amount = 2.10m,
                PlacedAtUtc = new DateTime(2025, 1, 12, 9, 5, 10, DateTimeKind.Utc)
            },
            new Bid
            {
                Id = 2,
                VeilingId = 2,
                VeilingProductId = 4,
                KoperId = 2,
                Amount = 3.80m,
                PlacedAtUtc = new DateTime(2025, 1, 13, 10, 2, 8, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<AuditEntry>().HasData(
            new AuditEntry
            {
                Id = 1,
                VeilingId = 1,
                Action = "Seed: Veiling 1 aangemaakt",
                CreatedAtUtc = createdUtc,
                ActorGebruikerId = 3
            },
            new AuditEntry
            {
                Id = 2,
                VeilingId = 2,
                Action = "Seed: Veiling 2 aangemaakt",
                CreatedAtUtc = createdUtc,
                ActorGebruikerId = 4
            }
        );
    }
}
