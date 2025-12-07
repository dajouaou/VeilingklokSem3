using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Veiling
{
    public int Id { get; set; } //pk

    public string Naam { get; set; } = string.Empty; //naam van de veiling

    public int VMId { get; set; } //fk
    public VM? VM { get; set; } //vm zelf

    public VeilingStatus Status { get; set; } = VeilingStatus.Draft; //status

    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? Locatie { get; set; } //locatie van v

    public int? CurrentVeilingProductId { get; set; }
    public VeilingProduct? CurrentVeilingProduct { get; set; }

    public List<VeilingProduct> VeilingProducten { get; set; } = new();
    public List<Bid> Bids { get; set; } = new();
    public List<AuditEntry> AuditEntries { get; set; } = new();

    public byte[]? RowVersion { get; set; }
}