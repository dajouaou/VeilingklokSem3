public sealed class VeilingProductDto
{
    public int Id { get; set; }
    public int VeilingId { get; set; }
    public int ProductId { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string? Categorie { get; set; }
    public string? Kleur { get; set; }
    public string? Hoogte { get; set; }
    public int? AantalPerBos { get; set; }
    public int Volgorde { get; set; }
    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal MinimumPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public decimal? LaatsteBodBedrag { get; set; }
    public int BiedCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ClockDurationMs { get; set; }
    public int ClockTickMs { get; set; }
    public decimal PriceDropPerTick { get; set; }
    public int? SoldToKoperId { get; set; }
}