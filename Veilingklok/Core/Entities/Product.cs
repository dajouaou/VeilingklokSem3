namespace Veilingklok.Core.Entities;

public class Product
{
    public int Id { get; set; }                 // PK

    public int AanvoerderId { get; set; }       // FK
    public string Naam { get; set; } = "";
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public string? FotoUrl { get; set; }

    public Aanvoerder? Aanvoerder { get; set; }

    public List<VeilingProduct> VeilingProducten { get; set; } = new();
}