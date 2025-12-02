namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Lot;

public sealed class CurrentLotDto
{
    public int VeilingProductId { get; set; }

    // Product info
    public int ProductId { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }

    // Veiling info
    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }

    public decimal? LaatsteBod { get; set; }
    public int BidCount { get; set; }

    public string Status { get; set; } = string.Empty;

    // Aanvoerder
    public int AanvoerderId { get; set; }
    public string AanvoerderNaam { get; set; } = string.Empty;
}