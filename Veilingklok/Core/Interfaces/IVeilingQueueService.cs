using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Queue;

namespace Veilingklok.Core.Interfaces;

public interface IVeilingQueueService
{
    Task<List<QueueGroupDto>> GetQueueAsync(int veilingId, bool onlyQueued);
    Task<Result> AddQueueItemAsync(int veilingId, AddQueueItemDto dto, int actorGebruikerId);
    Task<Result> ReorderQueueAsync(int veilingId, ReorderQueueDto dto, int actorGebruikerId);
}