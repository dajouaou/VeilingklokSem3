namespace Veilingklok.Core.Entities
{
    public class Veilingmeester
    {
        public int Id { get; set; }                 // PK
        public int GebruikerId { get; set; }        // FK → Gebruiker
        public string Naam { get; set; } = string.Empty;

        // Navigatie
        public Gebruiker? Gebruiker { get; set; }

    }
}
