namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Audit;

public sealed class AuditEntryDto
{
    public int Id { get; set; }
    public int VeilingId { get; set; }

    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public string ActorNaam { get; set; } = string.Empty;
}