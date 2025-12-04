namespace Veilingklok.Core.Entities;

public class Koper
{
    public int Id { get; set; }                 // PK
    public int GebruikerId { get; set; }        // FK
    public string Email { get; set; } = "";
    public string Naam { get; set; } = "";

    public Gebruiker? Gebruiker { get; set; }

    public List<Bid> Bids { get; set; } = new();
    public List<VeilingProduct> GekochteVeilingProducten { get; set; } = new();
}