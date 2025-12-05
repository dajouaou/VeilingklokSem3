using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class VeilingBidResultDto
{
    public int BidId { get; set; }
    public decimal Amount { get; set; }

    public static VeilingBidResultDto FromEntity(Bid bid)
    {
        return new VeilingBidResultDto
        {
            BidId = bid.Id,
            Amount = bid.Amount
        };
    }
}