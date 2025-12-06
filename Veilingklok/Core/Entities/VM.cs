namespace Veilingklok.Core.Entities;

public class VM
{
    public int Id { get; set; }//pk

    public int GebruikerId { get; set; }//fk naar gebruiker
    public Gebruiker? Gebruiker { get; set; }//navigatie naar gebruiker

    public string Naam { get; set; } = string.Empty;//naam van vm

    public List<Veiling> Veilingen { get; set; } = new();//alle v 
}