using System;

namespace Veilingklok.Core.Entities;

public class AuditEntry
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling Veiling { get; set; } = null!;

    public string Action { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int ActorGebruikerId { get; set; }
    public Gebruiker? ActorGebruiker { get; set; }
}