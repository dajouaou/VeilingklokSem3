namespace Veilingklok.Features.AanvoerderDashboard.Dtos
{
    public class AanmeldingListItemDto
    {
        public int Id { get; set; }

        public string Soort { get; set; } = string.Empty;
        public string? PotmaatOfSteellengte { get; set; }

        public int Hoeveelheid { get; set; }
        public decimal MinimumPrijs { get; set; }

        public string KlokLocatie { get; set; } = string.Empty;
        public DateTime Veildatum { get; set; }

        public string? FotoUrl { get; set; }

        // Verkoopinfo
        public bool IsVerkocht { get; set; }
        public string? KoperNaam { get; set; }
        public decimal? VerkoopPrijs { get; set; }
        public decimal? TotaleOpbrengst { get; set; }
    }
}
