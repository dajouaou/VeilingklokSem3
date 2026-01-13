namespace Veilingklok.Features.AanvoerderDashboard.Dtos
{
    public class AanvoerderStatsDto
    {

        public int TotaalAantalAanmeldingen { get; set; }

        public int AantalVerkocht { get; set; }

        // Totale opbrengst van verkochte items
        public decimal TotaleOpbrengst { get; set; }
    }
}
