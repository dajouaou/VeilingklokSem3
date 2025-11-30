namespace Veilingklok.Infrastructure.SignalR.Events;

//statuswijzigingen
public record AuctionStatusChangedEvent(int VeilingId, string NewStatus);