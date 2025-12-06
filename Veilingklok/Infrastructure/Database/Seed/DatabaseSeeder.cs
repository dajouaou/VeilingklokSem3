using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Auth.Services;

namespace Veilingklok.Infrastructure.Database.Seed;

public static class DatabaseSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // -----------------------------------------
        // 0. Password hashing (nodig voor seed users)
        // -----------------------------------------
        var p = new PasswordService();

        string hashVm      = p.HashPassword("test");
        string hashAanv    = p.HashPassword("test");
        string hashKoper   = p.HashPassword("test");

        // -----------------------------------------
        // 1. GEBRUIKERS
        // -----------------------------------------
        modelBuilder.Entity<Gebruiker>().HasData(
            new Gebruiker
            {
                Id = 1,
                Username     = "vm1",
                Email        = "vm1@example.com",
                PasswordHash = hashVm,
                Voornaam     = "Veiling",
                Achternaam   = "Meester",
                Role         = UserRole.VM
            },
            new Gebruiker
            {
                Id = 2,
                Username     = "aanvoerder1",
                Email        = "aanvoerder1@example.com",
                PasswordHash = hashAanv,
                Voornaam     = "Jan",
                Achternaam   = "Aanvoerder",
                Role         = UserRole.Aanvoerder
            },
            new Gebruiker
            {
                Id = 3,
                Username     = "koper1",
                Email        = "koper1@example.com",
                PasswordHash = hashKoper,
                Voornaam     = "Klaas",
                Achternaam   = "Koper",
                Role         = UserRole.Koper
            }
        );

        // -----------------------------------------
        // 2. VM / Aanvoerder / Koper koppelingen
        // -----------------------------------------
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

        // -----------------------------------------
        // 3. PRODUCTEN
        // -----------------------------------------
        modelBuilder.Entity<Product>().HasData(
            new Product {
                Id = 1,
                Naam = "Alstroemeria Mix",
                Categorie = "Bloemen",
                Beschrijving = "Frisse mix alstroemeria's",
                FotoUrl = "/images/products/alstroemeria-mix.jpg",
                AanvoerderId = 1,
                Soort = "Bloem",
                PotmaatOfSteellengte = "60cm",
                HoeveelheidStuks = 50,
                MinimumPrijs = 4m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = DateTime.UtcNow
            },
            new Product {
                Id = 2,
                Naam = "Boeket Gerbera Mix",
                Categorie = "Boeket",
                Beschrijving = "Vrolijke mix van gerbera's",
                FotoUrl = "/images/products/boeket-gerbera-mix.jpg",
                AanvoerderId = 1,
                Soort = "Boeket",
                PotmaatOfSteellengte = "n.v.t.",
                HoeveelheidStuks = 30,
                MinimumPrijs = 5m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = DateTime.UtcNow
            },
            new Product {
                Id = 3,
                Naam = "Orchidee Phalaenopsis Wit",
                Categorie = "Planten",
                Beschrijving = "Elegante witte orchidee",
                FotoUrl = "/images/products/orchidee-phalaenopsis-wit.jpg",
                AanvoerderId = 1,
                Soort = "Plant",
                PotmaatOfSteellengte = "12cm pot",
                HoeveelheidStuks = 20,
                MinimumPrijs = 12m,
                KlokLocatie = "Aalsmeer",
                VeilDatum = DateTime.UtcNow
            }
        );

        // -----------------------------------------
        // 4. VEILING
        // -----------------------------------------
        modelBuilder.Entity<Veiling>().HasData(
            new Veiling
            {
                Id = 1,
                VMId = 1,
                Status = VeilingStatus.Scheduled,
                Locatie = "Aalsmeer",
                StartTijdUtc = DateTime.UtcNow.AddMinutes(10),
                EindTijdUtc = DateTime.UtcNow.AddHours(2)
            }
        );

        // -----------------------------------------
        // 5. VEILING PRODUCTEN
        // -----------------------------------------
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
                Hoeveelheid = 30,
                StartPrijs = 12m,
                HuidigePrijs = 12m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 3,
                VeilingId = 1,
                ProductId = 3,
                AanvoerderId = 1,
                Volgorde = 3,
                Hoeveelheid = 20,
                StartPrijs = 20m,
                HuidigePrijs = 20m,
                Status = VeilingProductStatus.Queued
            }
        );
    }
}
