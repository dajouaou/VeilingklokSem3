namespace Veilingklok.Features.Veiling.Dtos
{
    public class BodPlaatsenDto
    {
        public int VeilingProductId { get; set; }
        public decimal Prijs { get; set; }

        // 0 of leeg => koop hele resterende partij
        public int Aantal { get; set; } = 0;
    }
}
