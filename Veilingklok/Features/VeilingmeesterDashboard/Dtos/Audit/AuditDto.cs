namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Audit;

public sealed class AuditDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int ActorGebruikerId { get; set; }
    public string ActorNaam { get; set; } = string.Empty;
}