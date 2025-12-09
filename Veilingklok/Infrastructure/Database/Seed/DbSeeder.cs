using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Features.Auth.Services;

namespace Veilingklok.Infrastructure.Database.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(MyContext db, PasswordService passwordService)
    {
        // Seeder draait alleen op lege database ? registratieformulier blijft werken
        if (await db.Gebruikers.AnyAsync()) return;

        var now = DateTime.UtcNow;

        string Hash(string pw) => passwordService.HashPassword(pw);

  
        var users = new List<Gebruiker>
        {

            new() {
                Email = "DLS.admin@jemid.com",
                PasswordHash = Hash("Pass123!"),
                Rol = UserRole.Admin,
                Voornaam = "DLS",
                Achternaam = "Admin",
                CreatedAtUtc = now
            },

            new() {
                Email = "sonia@veilingklok.local",
                PasswordHash = Hash("Pass123!"),
                Rol = UserRole.Aanvoerder,
                Voornaam = "Sonia",
                Achternaam = "Pasarga",
                CreatedAtUtc = now
            },

            new() {
                Email = "chris@veilingklok.local",
                PasswordHash = Hash("Pass123!"),
                Rol = UserRole.Koper,
                Voornaam = "Chris",
                Achternaam = "Shayan",
                CreatedAtUtc = now
            },

            new() {
                Email = "tristan@veilingklok.local",
                PasswordHash = Hash("Pass123!"),
                Rol = UserRole.Koper,
                Voornaam = "Tristan",
                Achternaam = "Perspolisi",
                CreatedAtUtc = now
            },

            new() {
                Email = "miriam@veilingklok.local",
                PasswordHash = Hash("Pass123!"),
                Rol = UserRole.Koper,
                Voornaam = "Miriam",
                Achternaam = "van de Hoeven",
                CreatedAtUtc = now
            }
        };

        db.Gebruikers.AddRange(users);
        await db.SaveChangesAsync();

        foreach (var user in users)
        {
            switch (user.Rol)
            {
                case UserRole.Aanvoerder:
                    db.Aanvoerders.Add(new Aanvoerder
                    {
                        GebruikerId = user.Id,
                        Naam = $"{user.Voornaam} {user.Achternaam}"
                    });
                    break;

                case UserRole.Koper:
                    db.Kopers.Add(new Koper
                    {
                        GebruikerId = user.Id,
                        Naam = $"{user.Voornaam} {user.Achternaam}"
                    });
                    break;

                case UserRole.Veilingmeester:
                    db.Veilingmeesters.Add(new Veilingmeester
                    {
                        GebruikerId = user.Id,
                        Naam = $"{user.Voornaam} {user.Achternaam}"
                    });
                    break;

            }
        }

        await db.SaveChangesAsync();
    }
}
