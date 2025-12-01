using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

/// <summary>
/// Een bod op een specifiek VeilingProduct (lot) binnen een Veiling.
/// Biedingen komen realtime binnen via SignalR. Dit model ondersteunt
/// zowel biedingen van kopers als biedingen van de veilingmeester zelf
/// (bv. startbod, prijsaanpassing).
/// </summary>
public class Bid
{
    // --------------------------------------------------------------------
    // Primary Key
    // --------------------------------------------------------------------
    public int Id { get; set; }                              // PK

    // --------------------------------------------------------------------
    // Relaties naar de veiling en het actieve lot
    // --------------------------------------------------------------------

    /// <summary>
    /// De veiling waarin dit bod is geplaatst.
    /// Nodig voor dashboard en auditlog.
    /// </summary>
    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    /// <summary>
    /// Het specifieke veiling-lot waarop wordt geboden.
    /// Nodig voor realtime updates van prijs en status.
    /// </summary>
    public int VeilingProductId { get; set; }
    public VeilingProduct? VeilingProduct { get; set; }

    // --------------------------------------------------------------------
    // Actor — wie heeft het bod daadwerkelijk ingediend?
    // --------------------------------------------------------------------

    /// <summary>
    /// De gebruiker die het bod heeft ingediend.
    /// Dit kan een Koper zijn of de Veilingmeester (bijv. systeemacties).
    /// </summary>
    public int PlacedByGebruikerId { get; set; }
    public Gebruiker? PlacedByGebruiker { get; set; }

    /// <summary>
    /// Indien het bod afkomstig is van een echte koper (niet systeem).
    /// Wordt null gelaten wanneer een veilingmeester of systeem een bod plaatst.
    /// </summary>
    public int? KoperId { get; set; }
    public Koper? Koper { get; set; }

    // --------------------------------------------------------------------
    // Biedinformatie
    // --------------------------------------------------------------------

    /// <summary>
    /// Het bedrag van het bod. Precision wordt via EF Fluent API ingesteld
    /// op decimal(18,2).
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Bron van het bod: Koper, Veilingmeester of Systeem.
    /// Nodig voor UI-logica en auditlogging.
    /// </summary>
    public BidSource Source { get; set; } = BidSource.Buyer;

    /// <summary>
    /// Tijdstip van het bod (UTC).  
    /// Belangrijk voor sortering, audit en concurrentiehandling.
    /// </summary>
    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;
}
