using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

public class AanmeldingUpdateDto
{
    // Soort product (verplicht)
    [Required, MaxLength(100)]
    public string Soort { get; set; } = string.Empty;

    // Optionele kenmerken
    public string? Potmaat { get; set; }
    public string? Steellengte { get; set; }

    // Aantal stuks, minimaal 1
    [Range(1, int.MaxValue)]
    public int Hoeveelheid { get; set; }

    // Minimumprijs per stuk
    [Range(0.01, double.MaxValue)]
    public decimal MinimumPrijs { get; set; }

    // Kloklocatie van de veiling
    [Required]
    public KlokLocatie KlokLocatie { get; set; }

    // Leverdatum van het product
    [Required]
    public DateTime LeverDatum { get; set; }

    // Nieuwe foto (optioneel)
    public IFormFile? Foto { get; set; }

    // Extra beschrijving
    public string? Beschrijving { get; set; }
}
