namespace Veilingklok.Core.Entities;

public class Product
{
    public int Id { get; set; }

    public int AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    public string Naam { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public string? FotoUrl { get; set; }

    public string? Kleur { get; set; }
    public string? Hoogte { get; set; }
    public int? AantalPerBos { get; set; }

    public List<VeilingProduct> VeilingProducten { get; set; } = new();
}
