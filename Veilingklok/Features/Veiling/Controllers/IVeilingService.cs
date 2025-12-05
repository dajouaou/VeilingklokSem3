using Veilingklok.Core.Entities;
using Veilingklok.Core.Shared;

namespace Veilingklok.Features.Veiling.Services;

public interface IVeilingService
{
    Task<Result<VeilingProduct>> GetCurrentProductAsync(int veilingId);
    Task<Result<List<VeilingProduct>>> GetQueueAsync(int veilingId);
    Task<Result<Bid>> PlaceBidAsync(int veilingId, int koperId, decimal amount);
}
