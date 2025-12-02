using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Bids;

namespace Veilingklok.Core.Interfaces;

public interface IVeilingBidService
{
    Task<List<BidDto>> GetBidsAsync(int veilingId, int? veilingProductId = null);
    Task<Result<BidDto>> PlaceBidAsync(int veilingId, PlaceBidDto dto);
}