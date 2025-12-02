public sealed class CreateVeilingProductDto
{
    public int VeilingId { get; set; }
    public int ProductId { get; set; }
    public int Volgorde { get; set; }
    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal MinimumPrijs { get; set; }
    public int ClockDurationMs { get; set; } = 10000;
    public int ClockTickMs { get; set; } = 100;
    public decimal PriceDropPerTick { get; set; } = 0.05m;
}