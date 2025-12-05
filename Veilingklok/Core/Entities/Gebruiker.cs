using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Gebruiker
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(64)]
    public string Voornaam { get; set; } = string.Empty;

    [Required, MaxLength(64)]
    public string Achternaam { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Koper;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;


    public Aanvoerder? Aanvoerder { get; set; }
    public Koper? Koper { get; set; }
    public VM? VM { get; set; }

    
    public string FullName => $"{Voornaam} {Achternaam}".Trim();
}
