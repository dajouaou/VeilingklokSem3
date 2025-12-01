using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

/// <summary>
/// De basisgebruiker van het systeem. 
/// Elke gebruiker heeft een login-account met rol-gebaseerde toegang
/// (koper, aanvoerder, veilingmeester, admin).
/// 
/// Profielen zoals Aanvoerder, Koper en Veilingmeester zijn 1-op-1
/// gekoppeld aan deze Gebruiker.
/// </summary>
public class Gebruiker
{
    // --------------------------------------------------------------------
    // Primary Key
    // --------------------------------------------------------------------
    public int Id { get; set; }                                      // PK

    // --------------------------------------------------------------------
    // Authenticatie / login gegevens
    // --------------------------------------------------------------------

    /// <summary>
    /// Unieke gebruikersnaam voor login.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Uniek e-mailadres, gebruikt voor login en communicatie.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gehasht wachtwoord (BCrypt). 
    /// Nooit plain text opslaan → NFR: beveiliging.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    // --------------------------------------------------------------------
    // Optionele profielinformatie
    // --------------------------------------------------------------------

    /// <summary>
    /// Weergavenaam van de gebruiker (optioneel, handig voor UI).
    /// </summary>
    public string? Naam { get; set; }

    // --------------------------------------------------------------------
    // Rolgebaseerde toegang (RBAC)
    // --------------------------------------------------------------------

    /// <summary>
    /// De rol bepaalt welke interface en functionaliteiten een gebruiker ziet.
    /// Koper, Aanvoerder, Veilingmeester, Admin.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Koper;

    /// <summary>
    /// Wanneer dit account is aangemaakt (UTC consistent).
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // --------------------------------------------------------------------
    // 1-op-1 profielrelaties (optioneel per rol)
    // --------------------------------------------------------------------

    /// <summary>
    /// Profielgegevens voor een Aanvoerder (kweker/aanbieder).
    /// </summary>
    public Aanvoerder? Aanvoerder { get; set; }

    /// <summary>
    /// Profielgegevens voor een Koper (biedt en koopt).
    /// </summary>
    public Koper? Koper { get; set; }

    /// <summary>
    /// Profielgegevens voor een Veilingmeester (stuurt veilingen aan).
    /// </summary>
    public Veilingmeester? Veilingmeester { get; set; }
}
