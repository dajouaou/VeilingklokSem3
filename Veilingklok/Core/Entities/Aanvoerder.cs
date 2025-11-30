namespace Veilingklok.Core.Entities;

public class Aanvoerder
{
    public int Id { get; set; }                 // PK 
    public int GebruikerId { get; set; }        // FK
    public string Naam { get; set; } = "";

    public Gebruiker? Gebruiker { get; set; }

    public List<Product> Producten { get; set; } = new();
    public List<VeilingProduct> VeilingProducten { get; set; } = new();
} 