using Microsoft.AspNetCore.SignalR;
using Veilingklok.Infrastructure.SignalR.Hubs;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Infrastructure.SignalR.Broadcasters
{
    public interface IVeilingBroadcastService
    {
        Task StuurHuidigProduct(int veilingId, HuidigProductDto product);
        Task StuurWachtrij(int veilingId, List<WachtrijItemDto> wachtrij);

        Task StuurBod(int veilingId, BodDto bod);
        Task StuurBiedingen(int veilingId, List<BodDto> biedingen);

        Task StuurAuditEvent(int veilingId, AuditEventDto audit);
        Task StuurAuditEvents(int veilingId, List<AuditEventDto> audits);
    }

    public class VeilingBroadcastService : IVeilingBroadcastService
    {
        private readonly IHubContext<AuctionHub> _hub;

        public VeilingBroadcastService(IHubContext<AuctionHub> hub)
        {
            _hub = hub;
        }

        private static string Groep(int veilingId) => $"veiling-{veilingId}";

        public Task StuurHuidigProduct(int veilingId, HuidigProductDto product)
            => _hub.Clients.Group(Groep(veilingId)).SendAsync("OntvangHuidigProduct", product);

        public Task StuurWachtrij(int veilingId, List<WachtrijItemDto> wachtrij)
            => _hub.Clients.Group(Groep(veilingId)).SendAsync("OntvangWachtrij", wachtrij);

        public Task StuurBod(int veilingId, BodDto bod)
            => _hub.Clients.Group(Groep(veilingId)).SendAsync("OntvangBod", bod);

        public Task StuurBiedingen(int veilingId, List<BodDto> biedingen)
            => _hub.Clients.Group(Groep(veilingId)).SendAsync("OntvangBiedingen", biedingen);

        public Task StuurAuditEvent(int veilingId, AuditEventDto audit)
            => _hub.Clients.Group(Groep(veilingId)).SendAsync("OntvangAudit", audit);

        public Task StuurAuditEvents(int veilingId, List<AuditEventDto> audits)
            => _hub.Clients.Group(Groep(veilingId)).SendAsync("OntvangAudits", audits);
    }
}
