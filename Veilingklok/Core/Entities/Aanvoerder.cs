namespace Veilingklok.Core.Entities;

public class Aanvoerder
{
    public int Id { get; set; }                 // Primaire sleutel
    public int GebruikerId { get; set; }        // Foreign key naar Gebruiker
    public string Naam { get; set; } = "";      // Weergavenaam van de aanvoerder

    public Gebruiker? Gebruiker { get; set; }   // Navigatie naar gekoppelde gebruiker (kan null zijn)
    public List<Aanmelding> Aanmeldingen { get; set; } = new(); // Alle aanmeldingen van deze aanvoerder
}
