namespace Veilingklok.Features.Producten.Dtos;

public sealed class CreateProductDto
{
    public int AanvoerderId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public List<string> FotoUrls { get; set; } = new();
    public string? Kleur { get; set; }
    public string? Hoogte { get; set; }
    public int? AantalPerBos { get; set; }
}