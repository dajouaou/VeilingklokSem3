namespace Veilingklok.Features.Producten.Dtos;

public sealed class ProductSummaryDto
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public string? FotoUrl { get; set; }   // thumbnail (eerste foto)
    public string AanvoerderNaam { get; set; } = string.Empty;
}