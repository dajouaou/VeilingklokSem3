using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Aanmelding
{
    public int Id { get; set; }

    public string ProductNaam { get; set; } = "";
    public string Soort { get; set; } = "";
    public string FotoUrl { get; set; } = "";
    public int Aantal { get; set; }
    public decimal MinimumPrijs { get; set; }

    public Veilinglocatie Locatie { get; set; }
    public DateTime VeilingDatum { get; set; }

    public int AanvoerderId { get; set; }
    public Aanvoerder Aanvoerder { get; set; }
}
