using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.Product.Dtos
{
    public record ProductReadDto(
         int Id,
         string Naam,
         int Hoeveelheid,
         int AanvoerderId,
         string? AanvoerderNaam,
         string? AfbeeldingUrl // <-- nieuw
     );

    public class ProductCreateDto
    {
        [Required, StringLength(100)] public string Naam { get; set; } = string.Empty;
        [Range(0, int.MaxValue)] public int Hoeveelheid { get; set; }
        [Required] public int AanvoerderId { get; set; }

        public string? AfbeeldingUrl { get; set; }  // <-- nieuw
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        [Required] public int Id { get; set; }
    }
}
