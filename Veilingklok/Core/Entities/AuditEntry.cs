namespace Veilingklok.Core.Entities;

public class AuditEntry
{
    public int Id { get; set; }                 // PK
    public int VeilingId { get; set; }          // FK
    public int ActorGebruikerId { get; set; }   // FK

    public string Action { get; set; } = "";    //  "AUCTION_START", "BID_PLACED"
    public string? Details { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Veiling? Veiling { get; set; }
    public Gebruiker? ActorGebruiker { get; set; }
} 