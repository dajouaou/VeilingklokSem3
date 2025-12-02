using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingProduct;

namespace Veilingklok.Core.Interfaces;

public interface IQueueService
{
    Task<Result<List<VeilingProductDto>>> GetQueueAsync(int veilingId);

    Task<Result<bool>> ReorderAsync(int veilingId, List<int> orderedIds);

    Task<Result<VeilingProductDto>> NextAsync(int veilingId);

    Task<Result<bool>> SkipAsync(int veilingProductId);
}