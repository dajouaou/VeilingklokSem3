namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO voor het plaatsen van een bod op een veilingproduct
    public class BodPlaatsenDto
    {
        public int VeilingProductId { get; set; }
        // Het ID van het veilingproduct waarop geboden wordt

        public decimal Prijs { get; set; }
        // De prijs waarvoor het bod wordt geplaatst

        // 0 of leeg = koop hele resterende partij
        public int Aantal { get; set; } = 0;
        // Aantal eenheden dat gekocht wordt, 0 betekent alles wat nog beschikbaar is
    }
}
