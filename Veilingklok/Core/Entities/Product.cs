namespace Veilingklok.Core.Entities;

public class Product
{
    public int Id { get; set; }                 // PK

    public int AanvoerderId { get; set; }       // FK
    public string Naam { get; set; } = "";
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public string? FotoUrl { get; set; }

   
    public string Soort { get; set; } = string.Empty;            // bv. Roos, Tulp
    public string? PotmaatOfSteellengte { get; set; }            // potmaat of steellengte
    public int HoeveelheidStuks { get; set; }                    // in stuks
    public decimal MinimumPrijs { get; set; }                    // min. prijs per partij
    public string KlokLocatie { get; set; } = string.Empty;      // Naaldwijk / Aalsmeer / ...
    public DateTime VeilDatum { get; set; }                      // datum waarop geveild wordt

    public Aanvoerder? Aanvoerder { get; set; }

}
