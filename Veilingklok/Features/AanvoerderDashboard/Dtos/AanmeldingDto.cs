namespace Veilingklok.Features.AanvoerdersDashboard.Dtos;

public class AanmeldingDto
{
    public int Id { get; set; }
    public string ProductNaam { get; set; } = "";
    public string Soort { get; set; } = "";
    public string FotoUrl { get; set; } = "";
    public int Aantal { get; set; }
    public decimal MinimumPrijs { get; set; }
    public string Locatie { get; set; } = "";
    public string VeilingDatum { get; set; } = "";
}
