using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;


namespace Veilingklok.Infrastructure.Database.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(MyContext db)
    {
        if (await db.Gebruikers.AnyAsync()) return;

        var now = DateTime.UtcNow;

        string Img(string file) => $"/img/products/{file}";
        string Hash(string pw) => BCrypt.Net.BCrypt.HashPassword(pw);

        // 1) Gebruikers (10)

        var sofiaMeester = new Gebruiker
        {
            Username = "sofia.veilingmeester",
            Email = "sofia.veilingmeester@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Veilingmeester,
            CreatedAtUtc = now
        };

        var sofiaAdmin = new Gebruiker
        {
            Username = "sofia.admin",
            Email = "sofia.admin@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Admin,
            CreatedAtUtc = now
        };

        var saraUser = new Gebruiker
        {
            Username = "sara.aanvoerder",
            Email = "sara@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Aanvoerder,
            CreatedAtUtc = now
        };

        var soniaUser = new Gebruiker
        {
            Username = "sonia.aanvoerder",
            Email = "sonia@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Aanvoerder,
            CreatedAtUtc = now
        };

        var siaUser = new Gebruiker
        {
            Username = "sia.aanvoerder",
            Email = "sia@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Aanvoerder,
            CreatedAtUtc = now
        };

        var sashaUser = new Gebruiker
        {
            Username = "sasha.koper",
            Email = "sasha@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var saynaUser = new Gebruiker
        {
            Username = "sayna.koper",
            Email = "sayna@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var chrisUser = new Gebruiker
        {
            Username = "chris.koper",
            Email = "chris@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var tristanUser = new Gebruiker
        {
            Username = "tristan.koper",
            Email = "tristan@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var miriamUser = new Gebruiker
        {
            Username = "miriam.koper",
            Email = "miriam@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        db.Gebruikers.AddRange(
            sofiaMeester, sofiaAdmin,
            saraUser, soniaUser, siaUser,
            sashaUser, saynaUser, chrisUser, tristanUser, miriamUser
        );
        await db.SaveChangesAsync();

        // -----------------------------
        // 2) Profielen (3 + 5)
        // -----------------------------
        var saraAanvoerder = new Aanvoerder { GebruikerId = saraUser.Id, Naam = "Sara Pasargad" };
        var soniaAanvoerder = new Aanvoerder { GebruikerId = soniaUser.Id, Naam = "Sonia Pasarga" };
        var siaAanvoerder = new Aanvoerder { GebruikerId = siaUser.Id, Naam = "Sia Zagros" };

        var sashaKoper = new Koper { GebruikerId = sashaUser.Id, Naam = "Sasha Niki" };
        var saynaKoper = new Koper { GebruikerId = saynaUser.Id, Naam = "Sayna Shayan" };
        var chrisKoper = new Koper { GebruikerId = chrisUser.Id, Naam = "Chris Shayan" };
        var tristanKoper = new Koper { GebruikerId = tristanUser.Id, Naam = "Tristan Perspolisi" };
        var miriamKoper = new Koper { GebruikerId = miriamUser.Id, Naam = "Miriam van de Hoeven" };

        db.Aanvoerders.AddRange(saraAanvoerder, soniaAanvoerder, siaAanvoerder);
        db.Kopers.AddRange(sashaKoper, saynaKoper, chrisKoper, tristanKoper, miriamKoper);
        await db.SaveChangesAsync();
    }

}

