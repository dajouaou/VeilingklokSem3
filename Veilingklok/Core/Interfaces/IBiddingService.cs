// Veilingklok/Features/Veiling/Services/IBiddingService.cs
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Services;

public interface IBiddingService
{
    Task<Result<BidResultDto>> PlaceBidAsync(int veilingId, int gebruikerId, decimal? price);
}