namespace Veilingklok.Core.Entities;

/// <summary>
/// Logt belangrijke acties binnen een veiling, zoals het starten van een veiling,
/// het activeren van een lot, het plaatsen van een bod, het sluiten van een lot, enz.
/// Essentieel voor betrouwbaarheid, audits, diagnostiek en juridische traceerbaarheid.
/// </summary>
public class AuditEntry
{
    // --------------------------------------------------------------------
    // Primary Key
    // --------------------------------------------------------------------
    public int Id { get; set; }                                 // PK

    // --------------------------------------------------------------------
    // Relatie naar Veiling (context van de actie)
    // --------------------------------------------------------------------
    public int VeilingId { get; set; }                          // FK → Veiling
    public Veiling? Veiling { get; set; }

    // --------------------------------------------------------------------
    // Actor (de gebruiker die de actie uitvoerde)
    // Kan een veilingmeester, koper of andere rol zijn.
    // --------------------------------------------------------------------
    public int ActorGebruikerId { get; set; }                   // FK → Gebruiker
    public Gebruiker? ActorGebruiker { get; set; }

    // --------------------------------------------------------------------
    // Inhoud van de audit entry
    // --------------------------------------------------------------------

    /// <summary>
    /// Het type actie, zoals:
    /// AUCTION_START, AUCTION_END, LOT_ACTIVATED, LOT_CLOSED,
    /// BID_PLACED, PRICE_UPDATED, HEARTBEAT, etc.
    /// Wordt door backend-services consistent gevuld.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Optionele extra informatie over de actie.
    /// Kan JSON, extra tekst of metadata zijn.
    /// </summary>
    public string? Details { get; set; }

    // --------------------------------------------------------------------
    // Tijdstip
    // --------------------------------------------------------------------

    /// <summary>
    /// Tijdstip van de actie in UTC.
    /// Altijd UTC → consistent over alle browsers, servers en tijdzones.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
