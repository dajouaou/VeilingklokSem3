namespace Veilingklok.Core.Entities;

/// <summary>
/// Profiel voor een gebruiker met de rol "Veilingmeester".
/// Eén-op-één gekoppeld aan Gebruiker.
/// Verantwoordelijk voor het leiden van één of meerdere veilingen.
/// </summary>
public class Veilingmeester
{
    // ------------------------------------------------------------
    // Primary Key
    // ------------------------------------------------------------
    public int Id { get; set; }                        // PK

    // ------------------------------------------------------------
    // 1-op-1 relatie naar Gebruiker (auth + RBAC)
    // ------------------------------------------------------------
    public int GebruikerId { get; set; }              // FK → Gebruiker
    public Gebruiker? Gebruiker { get; set; }

    /// <summary>
    /// Weergavenaam van deze veilingmeester
    /// (optioneel — als je Gebruiker.Naam gebruikt kan dit desnoods weg).
    /// </summary>
    public string Naam { get; set; } = string.Empty;

    // ------------------------------------------------------------
    // Relaties
    // ------------------------------------------------------------

    /// <summary>
    /// Alle veilingen die door deze veilingmeester worden geleid.
    /// Nodig voor dashboard, beheer en analytics.
    /// </summary>
    public List<Veiling> Veilingen { get; set; } = new();
}
