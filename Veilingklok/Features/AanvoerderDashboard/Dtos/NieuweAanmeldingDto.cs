using Veilingklok.Core.Enums;

namespace Veilingklok.Features.AanvoerdersDashboard.Dtos;

public class NieuweAanmeldingDto
{
    public string ProductNaam { get; set; } = "";
    public string Soort { get; set; } = "";
    public string FotoUrl { get; set; } = "";
    public int Aantal { get; set; }
    public decimal MinimumPrijs { get; set; }
    public Veilinglocatie Locatie { get; set; }
    public DateTime VeilingDatum { get; set; }
    public int AanvoerderId { get; set; }
}
