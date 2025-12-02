namespace Veilingklok.Features.Producten.Dtos;

public sealed class UpdateProductDto
{
    public string Naam { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }

    public List<string> FotoUrls { get; set; } = new();

    public string? Kleur { get; set; }
    public string? Hoogte { get; set; }
    public int? AantalPerBos { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}