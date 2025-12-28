using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Infrastructure.SignalR.Hubs;

namespace Veilingklok.Infrastructure.SignalR.Broadcasters;

public sealed class VeilingBroadcastService : IVeilingBroadcastService
{
    private readonly IHubContext<AuctionPublicHub> _publicHub;
    private readonly IHubContext<AuctionPrivateHub> _privateHub;

    public VeilingBroadcastService(IHubContext<AuctionPublicHub> publicHub, IHubContext<AuctionPrivateHub> privateHub)
    {
        _publicHub = publicHub;
        _privateHub = privateHub;
    }

    private static string Groep(int veilingId) => $"veiling-{veilingId}";

    private Task SendToAll(int veilingId, string method, object payload)
    {
        var group = Groep(veilingId);
        var a = _publicHub.Clients.Group(group).SendAsync(method, payload);
        var b = _privateHub.Clients.Group(group).SendAsync(method, payload);
        return Task.WhenAll(a, b);
    }

    public Task StuurHuidigProduct(int veilingId, HuidigProductDto product)
        => SendToAll(veilingId, "OntvangHuidigProduct", product);

    public Task StuurWachtrij(int veilingId, List<WachtrijItemDto> wachtrij)
        => SendToAll(veilingId, "OntvangWachtrij", wachtrij);

    public Task StuurBod(int veilingId, BodDto bod)
        => SendToAll(veilingId, "OntvangBod", bod);

    public Task StuurBiedingen(int veilingId, List<BodDto> biedingen)
        => SendToAll(veilingId, "OntvangBiedingen", biedingen);

    public Task StuurAuditEvent(int veilingId, VMAuditDto audit)
        => _privateHub.Clients.Group(Groep(veilingId)).SendAsync("OntvangAudit", audit);

    public Task StuurAuditEvents(int veilingId, List<VMAuditDto> audits)
        => _privateHub.Clients.Group(Groep(veilingId)).SendAsync("OntvangAudits", audits);

    public Task StuurTick(int veilingId, TickDto tick)
        => SendToAll(veilingId, "OntvangTick", tick);
}
