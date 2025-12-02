namespace Veilingklok.Features.AanvoerderDashboard.Dtos;

// wordt gebruikt door admin om een aanvoerder-profiel toe te voegen aan een user
// userId komt UIT JWT in controller → niet uit DTO (veilig)
public sealed class CreateAanvoerderDto
{
    public string Naam { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
}