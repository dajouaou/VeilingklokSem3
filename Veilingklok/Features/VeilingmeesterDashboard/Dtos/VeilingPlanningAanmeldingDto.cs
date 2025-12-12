public class VeilingPlanningAanmeldingDto
{
    public int Id { get; set; }
    public string Soort { get; set; } = "";
    public int Hoeveelheid { get; set; }
    public decimal MinimumPrijs { get; set; }
    public string AanvoerderNaam { get; set; } = "";
    public DateTime LeverDatum { get; set; } 
}
