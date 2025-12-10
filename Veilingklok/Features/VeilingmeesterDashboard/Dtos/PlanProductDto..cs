namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class PlanProductDto
    {
        public int Id { get; set; }
        public string Soort { get; set; } = "";
        public int Hoeveelheid { get; set; }
        public string AanvoerderNaam { get; set; } = "";
        public string? FotoUrl { get; set; }
    }
}
