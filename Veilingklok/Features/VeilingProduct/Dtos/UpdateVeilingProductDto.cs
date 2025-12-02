public sealed class UpdateVeilingProductDto
{
    public int? Volgorde { get; set; }
    public int? Hoeveelheid { get; set; }
    public decimal? StartPrijs { get; set; }
    public decimal? MinimumPrijs { get; set; }
    public decimal? HuidigePrijs { get; set; }
    public string? Status { get; set; }
    public bool? IsHandmatigGestopt { get; set; }
    public string? StatusReason { get; set; }
}