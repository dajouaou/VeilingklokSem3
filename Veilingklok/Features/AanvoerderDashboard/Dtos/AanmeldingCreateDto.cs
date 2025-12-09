using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

public class AanmeldingCreateDto
{
    [Required, MaxLength(100)]
    public string Soort { get; set; } = string.Empty;

    public string? Potmaat { get; set; }
    public string? Steellengte { get; set; }

    [Range(1, int.MaxValue)]
    public int Hoeveelheid { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal MinimumPrijs { get; set; }

    [Required]
    public KlokLocatie KlokLocatie { get; set; }

    [Required]
    public DateTime Veildatum { get; set; }
    public IFormFile? Foto { get; set; }
}
