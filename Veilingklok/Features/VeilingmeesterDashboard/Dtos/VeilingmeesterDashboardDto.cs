using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class VeilingmeesterDashboardDto
    {
        public VeilingOverzichtDto Overzicht { get; set; } = new();
        public List<BodDto> Biedingen { get; set; } = new();
        public List<AuditEventDto> AuditEvents { get; set; } = new();
    }
}
