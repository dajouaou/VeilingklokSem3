using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.AanvoerderDashboard.Dtos
{

    public record AanvoerderReadDto(int Id, int GebruikerId, string? Naam, string? ContactInfo);

    public class AanvoerderCreateDto
    {
        [Required] public int GebruikerId { get; set; }
        [Required, StringLength(80)] public string Naam { get; set; } = string.Empty;
        [StringLength(120)] public string? ContactInfo { get; set; }
    }

    public class AanvoerderUpdateDto : AanvoerderCreateDto
    {
        [Required] public int Id { get; set; }
    }
}





