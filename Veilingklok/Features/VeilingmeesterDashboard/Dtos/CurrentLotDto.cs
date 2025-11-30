namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class CurrentLotDto
{
    public int VeilingProductId { get; set; }

    public int ProductId { get; set; }
    public string ProductNaam { get; set; } = "";
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }
    public string? FotoUrl { get; set; }

    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }

    public decimal? LastBid { get; set; }
    public int BidCount { get; set; }

    public string Status { get; set; } = "";

    public int AanvoerderId { get; set; }
    public string AanvoerderNaam { get; set; } = "";
}