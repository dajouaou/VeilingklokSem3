// Veilingklok/Infrastructure/SignalR/Hubs/AuctionPrivateHub.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Veilingklok.Infrastructure.SignalR.Hubs;

[Authorize]
public sealed class AuctionPrivateHub : Hub
{
    public Task JoinVeiling(int veilingId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"veiling-{veilingId}");

    public Task LeaveVeiling(int veilingId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"veiling-{veilingId}");
}