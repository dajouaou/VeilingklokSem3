namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Auction;

public sealed class AuctionStatusChangedEvent
{
    public VeilingStatusDto Status { get; set; } = new();
}