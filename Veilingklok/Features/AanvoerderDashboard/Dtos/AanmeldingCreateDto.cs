using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.AanvoerderDashboard.Dtos
{
    public class AanmeldingCreateDto
    {
        [Required, MaxLength(100)]
        public string Soort { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? PotmaatOfSteellengte { get; set; }

        [Range(1, int.MaxValue)]
        public int Hoeveelheid { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal MinimumPrijs { get; set; }

        [Required]
        public KlokLocatie KlokLocatie { get; set; }

        [Required]
        public DateTime Veildatum { get; set; }

        [MaxLength(256)]
        public string? FotoUrl { get; set; }
    }
}
