using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Veilingklok.Infrastructure.SignalR.Hubs
{
    [Authorize]
    public class AuctionHub : Hub
    {
        public Task JoinVeilingGroep(int veilingId)
        {
            return Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"veiling-{veilingId}"
            );
        }

        public Task VerlaatVeilingGroep(int veilingId)
        {
            return Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"veiling-{veilingId}"
            );
        }
    }
}
