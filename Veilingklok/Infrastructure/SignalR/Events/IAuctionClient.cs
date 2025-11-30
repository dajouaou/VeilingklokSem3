using System.Threading.Tasks;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Infrastructure.SignalR;

public interface IAuctionClient
{
    Task AuctionStatusChanged(int veilingId, string newStatus);
    Task CurrentLotChanged(int veilingId, CurrentLotDto lot);
    Task QueueUpdated(int veilingId);
    Task BidPlaced(BidDto bid);
}