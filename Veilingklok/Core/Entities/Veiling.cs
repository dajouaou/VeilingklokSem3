using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

/// <summary>
/// Een veiling-sessie geleid door een Veilingmeester.
/// Bevat timing, status, actieve lot-pointer en alle gekoppelde loten/biedingen.
/// </summary>
public class Veiling
{
    // ------------------------------------------------------------
    // Primary Key
    // ------------------------------------------------------------
    public int Id { get; set; }                             // PK

    // ------------------------------------------------------------
    // Wie leidt deze veiling (FK naar Veilingmeester)
    // ------------------------------------------------------------
    public int VeilingmeesterId { get; set; }
    public Veilingmeester? Veilingmeester { get; set; }

    // ------------------------------------------------------------
    // Status & timing (case-eisen)
    // ------------------------------------------------------------
    public VeilingStatus Status { get; set; } = VeilingStatus.Draft;

    /// <summary>
    /// UTC starttijd (wordt ingesteld door de veilingmeester).
    /// </summary>
    public DateTime? StartTijdUtc { get; set; }

    /// <summary>
    /// UTC eindtijd (optioneel, wordt vaak berekend).
    /// </summary>
    public DateTime? EindTijdUtc { get; set; }

    /// <summary>
    /// Wanneer deze veiling is aangemaakt.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optionele locatie (wordt in UI getoond).
    /// </summary>
    public string? Locatie { get; set; }

    // ------------------------------------------------------------
    // Dashboard: actief lot (pointer)
    // Hiermee kan UI direct weten welk product nu draait.
    // ------------------------------------------------------------
    public int? CurrentVeilingProductId { get; set; }
    public VeilingProduct? CurrentVeilingProduct { get; set; }

    // ------------------------------------------------------------
    // Relaties
    // ------------------------------------------------------------

    /// <summary>
    /// Alle loten (VeilingProducten) die in deze veiling worden geveild.
    /// </summary>
    public List<VeilingProduct> VeilingProducten { get; set; } = new();

    /// <summary>
    /// Alle biedingen in deze veiling (ongeacht welk lot).
    /// </summary>
    public List<Bid> Bids { get; set; } = new();

    /// <summary>
    /// Audit logging voor veiligheid / bewijs / dashboard.
    /// </summary>
    public List<AuditEntry> AuditEntries { get; set; } = new();

    // ------------------------------------------------------------
    // Concurrency — BELANGRIJK voor 200ms real-time updates
    // ------------------------------------------------------------
    public byte[]? RowVersion { get; set; }
}
