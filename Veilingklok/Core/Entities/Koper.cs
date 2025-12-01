namespace Veilingklok.Core.Entities;

/// <summary>
/// Een koper in het systeem. Koper is een 1-op-1 profiel op basis van een Gebruiker.
/// Kopers kunnen bieden op veilingproducten en loten winnen.
/// </summary>
public class Koper
{
    // ------------------------------------------------------------
    // Primary Key
    // ------------------------------------------------------------
    public int Id { get; set; }                             // PK

    // ------------------------------------------------------------
    // Identiteit / Relatie naar Gebruiker (1-op-1 profiel)
    // ------------------------------------------------------------

    /// <summary>
    /// FK → Gebruiker. 
    /// Elke koper is precies één gebruiker; registreren/inloggen gebeurt via Gebruiker.
    /// </summary>
    public int GebruikerId { get; set; }
    public Gebruiker? Gebruiker { get; set; }

    /// <summary>
    /// Optionele display-naam van de koper (kan ook via Gebruiker.Naam).
    /// </summary>
    public string Naam { get; set; } = string.Empty;

    // ------------------------------------------------------------
    // Financieel (optioneel volgens case, handig voor uitbreidingen)
    // ------------------------------------------------------------

    /// <summary>
    /// Saldo van de koper. 
    /// Wordt alleen gebruikt als je betalingen of balansbeheer wilt simuleren.
    /// Decimal-precision wordt via EF Fluent API ingesteld → decimal(18,2).
    /// </summary>
    public decimal Saldo { get; set; } = 0m;

    // ------------------------------------------------------------
    // Biedingen / gewonnen producten
    // ------------------------------------------------------------

    /// <summary>
    /// Alle biedingen die deze koper heeft geplaatst binnen veilingen.
    /// Nodig voor audit, historiek en UI-profielpagina’s.
    /// </summary>
    public List<Bid> Bids { get; set; } = new();

    /// <summary>
    /// Alle VeilingProduct-en (loten) die deze koper heeft gewonnen.
    /// Wordt gevuld door VeilingProduct.SoldToKoperId.
    /// </summary>
    public List<VeilingProduct> GekochteVeilingProducten { get; set; } = new();
}
