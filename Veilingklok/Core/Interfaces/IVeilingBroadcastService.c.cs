using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VM.Dtos;

namespace Veilingklok.Infrastructure.SignalR.Broadcasters;

public interface IVeilingBroadcastService
{
    Task StuurHuidigProduct(int veilingId, HuidigProductDto product);
    Task StuurWachtrij(int veilingId, List<WachtrijItemDto> wachtrij);
    Task StuurBod(int veilingId, BodDto bod);
    Task StuurBiedingen(int veilingId, List<BodDto> biedingen);
    Task StuurAuditEvent(int veilingId, VMAuditDto audit);
    Task StuurAuditEvents(int veilingId, List<VMAuditDto> audits);
    Task StuurTick(int veilingId, TickDto tick);
}