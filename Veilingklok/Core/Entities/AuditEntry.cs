namespace Veilingklok.Core.Entities;

public class AuditEntry
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    public int ActorGebruikerId { get; set; }
    public Gebruiker? ActorGebruiker { get; set; }

    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}