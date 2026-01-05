namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class PlanVeilingItemDto
    {
        public int AanmeldingId { get; set; }
        public decimal MaximumPrijs { get; set; }
        public decimal DalingPerSeconde { get; set; } = 0.10m; // default
    }
}
