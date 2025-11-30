using Microsoft.AspNetCore.SignalR;

namespace Veilingklok.Infrastructure.SignalR;

public sealed class AuctionHub : Hub<IAuctionClient>
{
    private const string GroupPrefix = "auction-";

    // 1 plek voor group-naam, zelfde stijl als dispatcher
    public static string GroupName(int veilingId) => $"{GroupPrefix}{veilingId}";

    // client moet dit callen na connect
    public Task JoinAuctionGroup(int veilingId)
        => Groups.AddToGroupAsync(Context.ConnectionId, GroupName(veilingId));

    // opruimen
    public Task LeaveAuctionGroup(int veilingId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(veilingId));
}