namespace Veilingklok.Core.Entities;

public class VM
{
    public int Id { get; set; }

    public int GebruikerId { get; set; }
    public Gebruiker? Gebruiker { get; set; }

    public string Naam { get; set; } = string.Empty;

    public List<Veiling> Veilingen { get; set; } = new();
}