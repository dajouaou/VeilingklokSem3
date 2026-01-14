namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO die het momenteel actieve veilingproduct beschrijft
    public class HuidigProductDto
    {
        public int VeilingProductId { get; set; }
        // Uniek ID van het veilingproduct

        public string Soort { get; set; } = "";
        // Soort product 

        public string? FotoUrl { get; set; }
        // Optionele foto van het product

        public decimal MaximumPrijs { get; set; }
        // Startprijs van het product

        public decimal MinimumPrijs { get; set; }
        // Ondergrens waar de klok stopt

        public decimal HuidigePrijs { get; set; }
        // Actuele prijs op de klok

        public decimal DalingPerSeconde { get; set; }
        // Hoeveel de prijs per seconde daalt

        public int ResterendeHoeveelheid { get; set; }
        // Aantal eenheden dat nog beschikbaar is

        public bool IsActief { get; set; }
        // Geeft aan of dit product momenteel geveild wordt

        public bool IsVerkocht { get; set; }
        // Geeft aan of het product verkocht is

        public bool IsDoorgedraaid { get; set; }
        // Geeft aan of het product zonder verkoop is doorgedraaid

        public int AanvoerderId { get; set; }
        // ID van de aanvoerder van dit product

        public string AanvoerderNaam { get; set; } = "";
        // Naam van de aanvoerder
    }
}
