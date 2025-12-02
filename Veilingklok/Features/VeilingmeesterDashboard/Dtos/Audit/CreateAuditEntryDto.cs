using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Audit;

public sealed class CreateAuditEntryDto
{
    [Required]
    public int VeilingId { get; set; }

    [Required]
    public int ActorGebruikerId { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty;

    public string? Details { get; set; }
}