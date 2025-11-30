namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class AuditDto
{
    public int Id { get; set; }
    public string Action { get; set; } = "";
    public string? Details { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int ActorGebruikerId { get; set; }
    public string ActorNaam { get; set; } = "";
}