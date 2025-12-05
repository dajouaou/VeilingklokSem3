namespace Veilingklok.Core.Entities;

public class Aanvoerder
{
    public int Id { get; set; }

    public int GebruikerId { get; set; }
    public Gebruiker? Gebruiker { get; set; }

    public string Naam { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }

    public List<Product> Producten { get; set; } = new();
    public List<VeilingProduct> VeilingProducten { get; set; } = new();
}
