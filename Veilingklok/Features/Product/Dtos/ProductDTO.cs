namespace Veilingklok.Features.Producten.Dtos;

public sealed class ProductDto
{
    public int Id { get; set; }
    public int AanvoerderId { get; set; }

    public string Naam { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public string? FotoUrl { get; set; }

    public string? Kleur { get; set; }
    public string? Hoogte { get; set; }
    public int? AantalPerBos { get; set; }
}