using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Infrastructure.Database;

public static class ModelBuilderSeedExtensions
{
    public static void SeedVeilingEnVmDashboard(this ModelBuilder modelBuilder)
    {
       
        
        
        var created = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        
        
        
        modelBuilder.Entity<Gebruiker>().HasData(
            new Gebruiker { Id = 1, Email = "koper@demo.nl", PasswordHash = "DEMO_HASH", Voornaam = "Koper", Achternaam = "Demo", Rol = UserRole.Koper, CreatedAtUtc = created },
            new Gebruiker { Id = 2, Email = "vm@demo.nl", PasswordHash = "DEMO_HASH", Voornaam = "VM", Achternaam = "Demo", Rol = UserRole.VM, CreatedAtUtc = created },
            new Gebruiker { Id = 3, Email = "aanvoerder@demo.nl", PasswordHash = "DEMO_HASH", Voornaam = "Aanvoerder", Achternaam = "Demo", Rol = UserRole.Aanvoerder, CreatedAtUtc = created }
        );

        
            
            
        modelBuilder.Entity<Koper>().HasData(
            new Koper { Id = 1, GebruikerId = 1, Naam = "Koper Demo BV" }
        );

        modelBuilder.Entity<VM>().HasData(
            new VM { Id = 1, GebruikerId = 2, Naam = "Veilingmeester Demo" }
        );

        modelBuilder.Entity<Aanvoerder>().HasData(
            new Aanvoerder { Id = 1, GebruikerId = 3, Naam = "Aanvoerder Demo" }
        );

        
        
        
        modelBuilder.Entity<Aanmelding>().HasData(
            new Aanmelding
            {
                Id = 1,
                AanvoerderId = 1,
                Soort = "Rozen",
                Hoeveelheid = 100,
                MinimumPrijs = 1.50m,
                KlokLocatie = KlokLocatie.Naaldwijk,
                LeverDatum = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Unspecified),
                FotoUrl = "img/rozen.jpg",
                VeilingProductId = 1
            },
            new Aanmelding
            {
                Id = 2,
                AanvoerderId = 1,
                Soort = "Tulpen",
                Hoeveelheid = 80,
                MinimumPrijs = 1.20m,
                KlokLocatie = KlokLocatie.Naaldwijk,
                LeverDatum = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Unspecified),
                FotoUrl = "img/tulpen.jpg",
                VeilingProductId = 2
            }
        );

    
        
        
        modelBuilder.Entity<Veiling>().HasData(
            new Veiling
            {
                Id = 1,
                Naam = "Demo Veiling",
                Locatie = KlokLocatie.Naaldwijk,
                Datum = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Unspecified),
                StartTijd = new TimeSpan(9, 0, 0),
                Status = VeilingStatus.Scheduled,
                VMId = 1,
                CurrentVeilingProductId = null,
                StartTijdUtc = null,
                EindTijdUtc = null
            }
        );

        
        
        
        modelBuilder.Entity<VeilingProduct>().HasData(
            new VeilingProduct
            {
                Id = 1,
                VeilingId = 1,
                AanmeldingId = 1,
                AanvoerderId = 1,
                ProductId = null,
                Status = VeilingProductStatus.Queued,
                StartPrijs = 3.00m,
                HuidigePrijs = 3.00m,
                MinimumPrijs = 1.50m,
                DurationSeconds = 20,
                Hoeveelheid = 100,
                Volgorde = 1,
                ActivatedAtUtc = null,
                ClosedAtUtc = null,
                KoperId = null
                
                
                
            },
            new VeilingProduct
            {
                Id = 2,
                VeilingId = 1,
                AanmeldingId = 2,
                AanvoerderId = 1,
                ProductId = null,
                Status = VeilingProductStatus.Queued,
                StartPrijs = 2.50m,
                HuidigePrijs = 2.50m,
                MinimumPrijs = 1.20m,
                DurationSeconds = 20,
                Hoeveelheid = 80,
                Volgorde = 2,
                ActivatedAtUtc = null,
                ClosedAtUtc = null,
                KoperId = null
            }
        );
    }
}
