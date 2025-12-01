namespace Veilingklok.Core.Entities;

/// <summary>
/// Een aanvoerder (kweker/leverancier) die producten aanlevert voor veilingen.
/// 1-op-1 gekoppeld aan een Gebruiker.
/// </summary>
public class Aanvoerder
{
    public int Id { get; set; }                    // PK

    // --------------------------------------------------
    // Identiteit / koppeling met Gebruiker
    // --------------------------------------------------
    public int GebruikerId { get; set; }           // FK → Gebruiker
    public Gebruiker? Gebruiker { get; set; }      // 1-op-1 profiel

    // --------------------------------------------------
    // Basisgegevens van de aanvoerder
    // --------------------------------------------------
    public string Naam { get; set; } = string.Empty;  // Verplichte naam voor UI/groepering
    public string? ContactInfo { get; set; }          // Optioneel: telefoon/email/locatie

    // --------------------------------------------------
    // Relaties
    // --------------------------------------------------

    /// <summary>
    /// Alle producten die deze aanvoerder in het algemeen levert
    /// (basis-catalogus van de kweker).
    /// </summary>
    public List<Product> Producten { get; set; } = new();

    /// <summary>
    /// Alle veiling-loten (VeilingProduct) waarin deze aanvoerder
    /// als aanbieder optreedt in een specifieke veiling.
    /// Dit gebruik je o.a. om komende producten per aanvoerder te groeperen.
    /// </summary>
    public List<VeilingProduct> VeilingProducten { get; set; } = new();
}
