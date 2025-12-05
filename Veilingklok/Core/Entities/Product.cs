namespace Veilingklok.Core.Entities;

public class Product
{
    public int Id { get; set; }                 // PK

    public int AanvoerderId { get; set; }       // FK
    public string Naam { get; set; } = "";
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public string? FotoUrl { get; set; }


    public string Soort { get; set; } = string.Empty;           
    public string? PotmaatOfSteellengte { get; set; }            
    public int HoeveelheidStuks { get; set; }                    
    public decimal MinimumPrijs { get; set; }                    
    public string KlokLocatie { get; set; } = string.Empty;      
    public DateTime VeilDatum { get; set; }                      

    public Aanvoerder? Aanvoerder { get; set; }

}
