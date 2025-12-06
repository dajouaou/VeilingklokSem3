namespace Veilingklok.Features.Veiling.Dtos
{
    public class HuidigProductDto
    {
        public int VeilingProductId { get; set; }
        public string Soort { get; set; } = "";
        public string? FotoUrl { get; set; }
        public decimal StartPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }
        public int Hoeveelheid { get; set; }
        public bool IsActief { get; set; }
        public bool IsVerkocht { get; set; }
    }
}
