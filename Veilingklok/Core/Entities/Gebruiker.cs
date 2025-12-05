using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

public class Gebruiker
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;
    public UserRole Rol { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Koper? Koper { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }
    public Veilingmeester? Veilingmeester { get; set; }


}
