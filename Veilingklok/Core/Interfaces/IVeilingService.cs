using Veilingklok.Core.Entities;
using Veilingklok.Core.Shared;

namespace Veilingklok.Features.Veiling.Services;

public interface IVeilingService
{
    
    //Haalt het actieve product op
    Task<Result<VeilingProduct>> GetCurrentProductAsync(int veilingId);
    
    //Haalt de queue op
    Task<Result<List<VeilingProduct>>> GetQueueAsync(int veilingId);
    
    //een gebruiker laat een bod plaatsen
    Task<Result<Bid>> PlaceBidAsync(int veilingId, int koperId, decimal amount);
}