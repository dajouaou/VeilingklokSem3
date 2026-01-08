namespace Veilingklok.Features.Veiling.Dtos
{
    public class HuidigProductDto
    {
        public int VeilingProductId { get; set; }

        public string Soort { get; set; } = "";
        public string? FotoUrl { get; set; }

        public decimal MaximumPrijs { get; set; }
        public decimal MinimumPrijs { get; set; }
        public decimal HuidigePrijs { get; set; }

        public decimal DalingPerSeconde { get; set; }

        public int ResterendeHoeveelheid { get; set; }

        public bool IsActief { get; set; }
        public bool IsVerkocht { get; set; }
        public bool IsDoorgedraaid { get; set; }
        public int AanvoerderId { get; set; }
        public string AanvoerderNaam { get; set; } = "";

    }
}
