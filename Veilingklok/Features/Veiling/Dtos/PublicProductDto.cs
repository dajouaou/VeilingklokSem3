namespace Veilingklok.Features.Veiling.Dtos;

    public sealed class PublicProductDto
    {
        public int VeilingProductId { get; set; }
        public string Soort { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public decimal StartPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }
        public int Hoeveelheid { get; set; }
    }
