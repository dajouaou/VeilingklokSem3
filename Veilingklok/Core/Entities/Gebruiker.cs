using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Gebruiker
{
    // --------------------------------------------------------------------
    // Primary Key
    // --------------------------------------------------------------------
    public int Id { get; set; }                                      // PK

    // --------------------------------------------------------------------
    // Authenticatie login gegevens
    // --------------------------------------------------------------------

   
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;

    // --------------------------------------------------------------------
    //  profielinformatie samengevoegd uit master
    // --------------------------------------------------------------------
    public string? Naam { get; set; }
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;

    // --------------------------------------------------------------------
    // Rol
    // --------------------------------------------------------------------
    public UserRole Role { get; set; } = UserRole.Koper;

  
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // --------------------------------------------------------------------
    // 1op1 profielrelaties
    // --------------------------------------------------------------------
    public Aanvoerder? Aanvoerder { get; set; }
    public Koper? Koper { get; set; }
    public Veilingmeester? Veilingmeester { get; set; }
}
