public class AanmeldingListItemDto
{
  
    public int Id { get; set; }

    public string Soort { get; set; } = string.Empty;

    public string? Potmaat { get; set; }
    public string? Steellengte { get; set; }


    public int Hoeveelheid { get; set; }

    public decimal MinimumPrijs { get; set; }

    // Naam van de kloklocatie
    public string KlokLocatie { get; set; } = string.Empty;

    // Leverdatum van het product
    public DateTime LeverDatum { get; set; }

    // URL naar de productfoto
    public string? FotoUrl { get; set; }

    // Verkoopinformatie
    public bool IsVerkocht { get; set; }
    public string? KoperNaam { get; set; }
    public decimal? VerkoopPrijs { get; set; }
    public decimal? TotaleOpbrengst { get; set; }

    
    public string? Beschrijving { get; set; }

    public string AanvoerderNaam { get; set; }
}
