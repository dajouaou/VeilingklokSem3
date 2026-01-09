using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
namespace Veilingklok.Infrastructure.SignalR.Hubs;

[Authorize]
public class AuctionHub : Hub
{
    private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _connections
        = new();

    private readonly IVeilingBroadcastService _broadcast;

    public AuctionHub(IVeilingBroadcastService broadcast)
    {
        _broadcast = broadcast;
    }

    public async Task JoinVeilingGroep(int veilingId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"veiling-{veilingId}");

        var dict = _connections.GetOrAdd(veilingId, _ => new ConcurrentDictionary<string, byte>());
        dict[Context.ConnectionId] = 1;

        await _broadcast.StuurOnlineBieders(veilingId, dict.Count);
    }

    public async Task VerlaatVeilingGroep(int veilingId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"veiling-{veilingId}");

        if (_connections.TryGetValue(veilingId, out var dict))
        {
            dict.TryRemove(Context.ConnectionId, out _);
            await _broadcast.StuurOnlineBieders(veilingId, dict.Count);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var kv in _connections)
        {
            if (kv.Value.TryRemove(Context.ConnectionId, out _))
                await _broadcast.StuurOnlineBieders(kv.Key, kv.Value.Count);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
