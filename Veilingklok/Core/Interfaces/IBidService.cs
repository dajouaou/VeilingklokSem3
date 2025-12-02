using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Core.Interfaces;

public interface IBidService
{
    Task<Result<BidDto>> PlaceBidAsync(int veilingId, int userId, BidCreateDto dto);
    Task<Result<List<BidListItemDto>>> GetBidsForVeilingAsync(int veilingId);
}