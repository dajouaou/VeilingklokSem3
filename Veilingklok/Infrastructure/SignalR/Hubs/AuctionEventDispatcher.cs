using Microsoft.AspNetCore.SignalR;
using Veilingklok.Infrastructure.SignalR.Events;

namespace Veilingklok.Infrastructure.SignalR;

public interface IAuctionEventDispatcher
{
    Task Publish(AuctionStatusChangedEvent e);
    Task Publish(CurrentLotChangedEvent e);
    Task Publish(QueueUpdatedEvent e);
    Task Publish(BidPlacedEvent e);
}

public sealed class AuctionEventDispatcher : IAuctionEventDispatcher
{
    private readonly IHubContext<AuctionHub, IAuctionClient> _hub;

    public AuctionEventDispatcher(IHubContext<AuctionHub, IAuctionClient> hub)
        => _hub = hub;

    private static string Group(int veilingId) => AuctionHub.GroupName(veilingId); // 1 bron

    public Task Publish(AuctionStatusChangedEvent e)
        => _hub.Clients.Group(Group(e.VeilingId))
            .AuctionStatusChanged(e.VeilingId, e.NewStatus);

    public Task Publish(CurrentLotChangedEvent e)
        => _hub.Clients.Group(Group(e.VeilingId))
            .CurrentLotChanged(e.VeilingId, e.Lot);

    public Task Publish(QueueUpdatedEvent e)
        => _hub.Clients.Group(Group(e.VeilingId))
            .QueueUpdated(e.VeilingId);

    public Task Publish(BidPlacedEvent e)
        => _hub.Clients.Group(Group(e.VeilingId))
            .BidPlaced(e.Bid);
}