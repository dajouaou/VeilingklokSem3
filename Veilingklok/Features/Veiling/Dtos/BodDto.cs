namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO die een enkel bod in de veiling voorstelt
    public class BodDto
    {
        public int Id { get; set; }
        // Unieke identificatie van het bod

        public decimal Prijs { get; set; }
        // Het geboden bedrag

        public string? KoperNaam { get; set; }
        // Naam van de koper (kan null zijn bij anoniem of onbekend bod)

        public DateTime Tijdstip { get; set; }
        // Moment waarop het bod is uitgebracht
    }
}
