using Microsoft.AspNetCore.SignalR;
using Veilingklok.Infrastructure.SignalR.Hubs;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Infrastructure.SignalR.Broadcasters
{
    public interface IVeilingBroadcastService
    {
        Task StuurHuidigProduct(int veilingId, HuidigProductDto dto);
        Task StuurWachtrij(int veilingId, List<WachtrijItemDto> queue);
        Task StuurBod(int veilingId, BodDto bod);
        Task StuurAuditEvent(int veilingId, AuditEventDto evt);
    }

    public class VeilingBroadcastService : IVeilingBroadcastService
    {
        private readonly IHubContext<AuctionHub> _hub;

        public VeilingBroadcastService(IHubContext<AuctionHub> hub)
        {
            _hub = hub;
        }

        public Task StuurHuidigProduct(int veilingId, HuidigProductDto dto) =>
            _hub.Clients.Group($"veiling-{veilingId}")
                .SendAsync("ReceiveCurrentLot", dto);

        public Task StuurWachtrij(int veilingId, List<WachtrijItemDto> queue) =>
            _hub.Clients.Group($"veiling-{veilingId}")
                .SendAsync("ReceiveQueue", queue);

        public Task StuurBod(int veilingId, BodDto bod) =>
            _hub.Clients.Group($"veiling-{veilingId}")
                .SendAsync("ReceiveBid", bod);

        public Task StuurAuditEvent(int veilingId, AuditEventDto evt) =>
            _hub.Clients.Group($"veiling-{veilingId}")
                .SendAsync("ReceiveAudit", evt);
    }
}
