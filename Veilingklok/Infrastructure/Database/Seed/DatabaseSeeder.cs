// Veilingklok/Infrastructure/Database/Seed/DatabaseSeeder.cs
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Infrastructure.Database.Seed;

public static class DatabaseSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var baseDate = new DateTime(2025, 1, 1, 8, 0, 0, DateTimeKind.Utc);

        const string hashVm =
            "dm0tc2VlZC1zYWx0LTAwMA==.KRqeXLGmj1DryTFBWCId0sZltd6nURyxHIqdjAmoXIo=";

        const string hashAanvoerder =
            "YXYtc2VlZC1zYWx0LTAwMA==.tyfrGs1Ns/tRy5ChiEVBJlLHs3wqTLN3d24QNeJYzJs=";

        const string hashKoper =
            "a3Atc2VlZC1zYWx0LTAwMA==.2ymdEbIJ9G+5O0/M/fT6jrS2ZRHYV3pzBK/2nW/BYco=";

        modelBuilder.Entity<Gebruiker>().HasData(
            new Gebruiker
            {
                Id = 1,
                Username = "vm1",
                Email = "vm1@example.com",
                PasswordHash = hashVm,
                Voornaam = "Veiling",
                Achternaam = "Meester",
                Role = UserRole.VM,
                CreatedAtUtc = baseDate
            },
            new Gebruiker
            {
                Id = 2,
                Username = "aanvoerder1",
                Email = "aanvoerder1@example.com",
                PasswordHash = hashAanvoerder,
                Voornaam = "Jan",
                Achternaam = "Aanvoerder",
                Role = UserRole.Aanvoerder,
                CreatedAtUtc = baseDate
            },
            new Gebruiker
            {
                Id = 3,
                Username = "koper1",
                Email = "koper1@example.com",
                PasswordHash = hashKoper,
                Voornaam = "Klaas",
                Achternaam = "Koper",
                Role = UserRole.Koper,
                CreatedAtUtc = baseDate
            }
        );

        modelBuilder.Entity<VM>().HasData(new VM
        {
            Id = 1,
            GebruikerId = 1,
            Naam = "Veiling Meester"
        });

        modelBuilder.Entity<Aanvoerder>().HasData(new Aanvoerder
        {
            Id = 1,
            GebruikerId = 2,
            Naam = "Jan Aanvoerder",
            ContactInfo = "jan@aanvoerder.com"
        });

        modelBuilder.Entity<Koper>().HasData(new Koper
        {
            Id = 1,
            GebruikerId = 3,
            Naam = "Klaas Koper",
            Saldo = 500m
        });

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Naam = "Alstroemeria Mix",
                Categorie = "Bloemen",
                Beschrijving = "Frisse mix alstroemeria's",
                FotoUrl = "/img/products/alstroemeria-mix.jpg",
                AanvoerderId = 1,
                Soort = "Bloem",
                PotmaatOfSteellengte = "60cm",
                HoeveelheidStuks = 50,
                MinimumPrijs = 4m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 2,
                Naam = "Anthurium Rood",
                Categorie = "Planten",
                Beschrijving = "Dieprode anthurium",
                FotoUrl = "/img/products/anthurium-rood.jpg",
                AanvoerderId = 1,
                Soort = "Plant",
                PotmaatOfSteellengte = "14cm pot",
                HoeveelheidStuks = 15,
                MinimumPrijs = 8m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 3,
                Naam = "Boeket Gerbera Mix",
                Categorie = "Boeket",
                Beschrijving = "Vrolijke mix van gerbera's",
                FotoUrl = "/img/products/boeket-gerbera-mix.jpg",
                AanvoerderId = 1,
                Soort = "Boeket",
                PotmaatOfSteellengte = "n.v.t.",
                HoeveelheidStuks = 30,
                MinimumPrijs = 5m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 4,
                Naam = "Boeket Ranunculus Mix Premium",
                Categorie = "Boeket",
                Beschrijving = "Luxe ranunculus-mix premium",
                FotoUrl = "/img/products/boeket-ranunculus-mix-premium.jpg",
                AanvoerderId = 1,
                Soort = "Boeket",
                PotmaatOfSteellengte = "n.v.t.",
                HoeveelheidStuks = 25,
                MinimumPrijs = 9m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 5,
                Naam = "Calla Lily Oranje",
                Categorie = "Bloemen",
                Beschrijving = "Oranje calla lelies",
                FotoUrl = "/img/products/calla-lily-oranje.jpg",
                AanvoerderId = 1,
                Soort = "Bloem",
                PotmaatOfSteellengte = "55cm",
                HoeveelheidStuks = 40,
                MinimumPrijs = 7m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 6,
                Naam = "Ficus Lyrata",
                Categorie = "Planten",
                Beschrijving = "Grote ficus lyrata kamerplant",
                FotoUrl = "/img/products/ficus-lyrata.jpg",
                AanvoerderId = 1,
                Soort = "Plant",
                PotmaatOfSteellengte = "24cm pot",
                HoeveelheidStuks = 10,
                MinimumPrijs = 15m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 7,
                Naam = "Gypsophila Paniculata Wit",
                Categorie = "Bloemen",
                Beschrijving = "Luchtige witte gypsophila",
                FotoUrl = "/img/products/gypsophila-paniculata-wit.jpg",
                AanvoerderId = 1,
                Soort = "Bloem",
                PotmaatOfSteellengte = "70cm",
                HoeveelheidStuks = 40,
                MinimumPrijs = 3m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 8,
                Naam = "Orchidee Phalaenopsis Wit",
                Categorie = "Planten",
                Beschrijving = "Elegante witte orchidee",
                FotoUrl = "/img/products/orchidee-phalaenopsis-wit.jpg",
                AanvoerderId = 1,
                Soort = "Plant",
                PotmaatOfSteellengte = "12cm pot",
                HoeveelheidStuks = 20,
                MinimumPrijs = 12m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 9,
                Naam = "Sansevieria Zeylanica",
                Categorie = "Planten",
                Beschrijving = "Sterke sansevieria zeylanica",
                FotoUrl = "/img/products/sansevieria-zeylanica.jpg",
                AanvoerderId = 1,
                Soort = "Plant",
                PotmaatOfSteellengte = "17cm pot",
                HoeveelheidStuks = 25,
                MinimumPrijs = 10m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 10,
                Naam = "Seizoensboeket Herfst Mix",
                Categorie = "Boeket",
                Beschrijving = "Warm seizoensboeket in herfsttinten",
                FotoUrl = "/img/products/seizoensboeket-herfst-mix.jpg",
                AanvoerderId = 1,
                Soort = "Boeket",
                PotmaatOfSteellengte = "n.v.t.",
                HoeveelheidStuks = 30,
                MinimumPrijs = 14m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 11,
                Naam = "Tulpen Dubbelbloemig Mix",
                Categorie = "Bloemen",
                Beschrijving = "Dubbelbloemige tulpenmix",
                FotoUrl = "/img/products/tulpen-dubbelbloemig-mix.jpg",
                AanvoerderId = 1,
                Soort = "Bloem",
                PotmaatOfSteellengte = "40cm",
                HoeveelheidStuks = 50,
                MinimumPrijs = 6m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            },
            new Product
            {
                Id = 12,
                Naam = "White Rose Lisianthus Boeket",
                Categorie = "Boeket",
                Beschrijving = "Licht boeket met witte rozen en lisianthus",
                FotoUrl = "/img/products/White-Rose-Lisianthus-Bouquet.jpg",
                AanvoerderId = 1,
                Soort = "Boeket",
                PotmaatOfSteellengte = "n.v.t.",
                HoeveelheidStuks = 20,
                MinimumPrijs = 18m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = baseDate
            }
        );

        modelBuilder.Entity<Veiling>().HasData(
            new Veiling
            {
                Id = 1,
                Naam = "Ochtendveiling Aalsmeer",
                VMId = 1,
                Status = VeilingStatus.Scheduled,
                Locatie = "Aalsmeer",
                StartTijdUtc = baseDate.AddMinutes(10),
                EindTijdUtc = baseDate.AddHours(2),
                CreatedAtUtc = baseDate
            }
        );

        modelBuilder.Entity<VeilingProduct>().HasData(
            new VeilingProduct
            {
                Id = 1,
                VeilingId = 1,
                ProductId = 1,
                AanvoerderId = 1,
                Volgorde = 1,
                Hoeveelheid = 50,
                StartPrijs = 10m,
                HuidigePrijs = 10m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 2,
                VeilingId = 1,
                ProductId = 2,
                AanvoerderId = 1,
                Volgorde = 2,
                Hoeveelheid = 15,
                StartPrijs = 15m,
                HuidigePrijs = 15m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 3,
                VeilingId = 1,
                ProductId = 3,
                AanvoerderId = 1,
                Volgorde = 3,
                Hoeveelheid = 30,
                StartPrijs = 12m,
                HuidigePrijs = 12m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 4,
                VeilingId = 1,
                ProductId = 4,
                AanvoerderId = 1,
                Volgorde = 4,
                Hoeveelheid = 25,
                StartPrijs = 18m,
                HuidigePrijs = 18m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 5,
                VeilingId = 1,
                ProductId = 5,
                AanvoerderId = 1,
                Volgorde = 5,
                Hoeveelheid = 40,
                StartPrijs = 14m,
                HuidigePrijs = 14m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 6,
                VeilingId = 1,
                ProductId = 6,
                AanvoerderId = 1,
                Volgorde = 6,
                Hoeveelheid = 10,
                StartPrijs = 30m,
                HuidigePrijs = 30m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 7,
                VeilingId = 1,
                ProductId = 7,
                AanvoerderId = 1,
                Volgorde = 7,
                Hoeveelheid = 40,
                StartPrijs = 8m,
                HuidigePrijs = 8m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 8,
                VeilingId = 1,
                ProductId = 8,
                AanvoerderId = 1,
                Volgorde = 8,
                Hoeveelheid = 20,
                StartPrijs = 24m,
                HuidigePrijs = 24m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 9,
                VeilingId = 1,
                ProductId = 9,
                AanvoerderId = 1,
                Volgorde = 9,
                Hoeveelheid = 25,
                StartPrijs = 20m,
                HuidigePrijs = 20m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 10,
                VeilingId = 1,
                ProductId = 10,
                AanvoerderId = 1,
                Volgorde = 10,
                Hoeveelheid = 30,
                StartPrijs = 28m,
                HuidigePrijs = 28m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 11,
                VeilingId = 1,
                ProductId = 11,
                AanvoerderId = 1,
                Volgorde = 11,
                Hoeveelheid = 50,
                StartPrijs = 16m,
                HuidigePrijs = 16m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 12,
                VeilingId = 1,
                ProductId = 12,
                AanvoerderId = 1,
                Volgorde = 12,
                Hoeveelheid = 20,
                StartPrijs = 32m,
                HuidigePrijs = 32m,
                Status = VeilingProductStatus.Queued
            }
        );
    }
}
