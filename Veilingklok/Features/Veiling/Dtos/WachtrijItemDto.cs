namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO voor een product dat in de wachtrij van de veiling staat
    public class WachtrijItemDto
    {
        public int VeilingProductId { get; set; }
        // ID van het veilingproduct

        public int Volgorde { get; set; }
        // Positie van het product in de veilingvolgorde

        public string Soort { get; set; } = "";
        // Soort product

        public string? FotoUrl { get; set; }
        // Optionele foto van het product

        public decimal MaximumPrijs { get; set; }
        // Startprijs van dit product

        public decimal MinimumPrijs { get; set; }
        // Minimumprijs waar de klok kan stoppen

        public int ResterendeHoeveelheid { get; set; }
        // Aantal eenheden dat nog beschikbaar is

        public int AanvoerderId { get; set; }
        // ID van de aanvoerder

        public string AanvoerderNaam { get; set; } = "";
        // Naam van de aanvoerder
    }
}
