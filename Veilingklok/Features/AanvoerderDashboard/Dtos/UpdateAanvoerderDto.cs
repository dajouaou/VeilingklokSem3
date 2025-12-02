namespace Veilingklok.Features.AanvoerderDashboard.Dtos;

// gebruikt door admin/veilingmeester om naam/contactinfo bij te werken
public sealed class UpdateAanvoerderDto
{
    public string Naam { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
}