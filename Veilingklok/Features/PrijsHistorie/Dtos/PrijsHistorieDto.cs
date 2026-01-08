namespace Veilingklok.Features.PrijsHistorie.Dtos
{
    public class PrijsHistorieDto
    {
        public string Soort { get; set; } = "";

        public decimal? GemiddeldeAlleAanvoerders { get; set; }
        public List<PrijsPuntDto> Laatste10AlleAanvoerders { get; set; } = new();

        public decimal? GemiddeldeHuidigeAanvoerder { get; set; }
        public List<PrijsPuntDto> Laatste10HuidigeAanvoerder { get; set; } = new();
    }

    public class PrijsPuntDto
    {
        public decimal Prijs { get; set; }
        public DateTime Tijdstip { get; set; }
        public string? AanvoerderNaam { get; set; }
    }
}
