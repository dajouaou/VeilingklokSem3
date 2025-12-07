namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class AuditEventDto
    {
        public string Gebeurtenis { get; set; } = "";
        public DateTime Tijdstip { get; set; } = DateTime.UtcNow;
        public string? UitgevoerdDoor { get; set; }
    }
}
