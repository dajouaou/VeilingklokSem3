using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Infrastructure.SignalR.Events;

public record BidPlacedEvent(int  VeilingId, BidDto Bid);