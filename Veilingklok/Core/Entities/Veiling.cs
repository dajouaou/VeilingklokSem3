using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Veiling
{
    public int Id { get; set; }

    public int VeilingmeesterId { get; set; }
    public Veilingmeester? Veilingmeester { get; set; }

    public VeilingStatus Status { get; set; } = VeilingStatus.Draft;

    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? Locatie { get; set; }

    public int? CurrentVeilingProductId { get; set; }
    public VeilingProduct? CurrentVeilingProduct { get; set; }

    public List<VeilingProduct> VeilingProducten { get; set; } = new();
    public List<Bid> Bids { get; set; } = new();
    public List<AuditEntry> AuditEntries { get; set; } = new();

    public byte[]? RowVersion { get; set; }
}
