public sealed class CurrentLotDto
{
    public int VeilingProductId { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public int Hoeveelheid { get; set; }
    public decimal HuidigePrijs { get; set; }
    public decimal MinimumPrijs { get; set; }
    public string? AanvoerderNaam { get; set; }
    public string Status { get; set; } = string.Empty;
}