namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Bids;

public sealed class BidPlacedEvent
{
    public BidDto Bid { get; set; } = new();
}