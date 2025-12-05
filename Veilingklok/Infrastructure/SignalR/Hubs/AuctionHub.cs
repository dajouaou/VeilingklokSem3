using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Veilingklok.Infrastructure.SignalR.Hubs
{
    [Authorize] // alleen ingelogde gebruikers
    public class AuctionHub : Hub
    {
        // Optioneel: log wanneer iemand connect
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        // Veilingmeester kan clients in een "veiling-room" laten joinen
        public async Task JoinVeilingGroup(int veilingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"veiling-{veilingId}");
        }

        public async Task LeaveVeilingGroup(int veilingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"veiling-{veilingId}");
        }
    }
}
