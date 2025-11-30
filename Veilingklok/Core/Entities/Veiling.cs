using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Veiling
{
    public int Id { get; set; } // PK

    public VeilingStatus Status { get; set; } = VeilingStatus.Draft;

    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // ------------------------------------------------------------
    // Dashboard: pointer naar het huidige lot in de veiling
    // ------------------------------------------------------------
    public int? CurrentVeilingProductId { get; set; }

    // Navigatie erbij voor dashboard queries
    public VeilingProduct? CurrentVeilingProduct { get; set; }

    // ------------------------------------------------------------
    // Relaties
    // ------------------------------------------------------------
    public List<VeilingProduct> VeilingProducten { get; set; } = new();
    public List<Bid> Bids { get; set; } = new();
    public List<AuditEntry> AuditEntries { get; set; } = new();
}