using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingProduct;


namespace Veilingklok.Core.Interfaces;

public interface IVeilingProductService
{
    Task<Result<VeilingProductDto>> CreateAsync(CreateVeilingProductDto dto);
    Task<Result<VeilingProductDto>> GetByIdAsync(int id);
    Task<Result<List<VeilingProductDto>>> GetByVeilingAsync(int veilingId);
    Task<Result<VeilingProductDto>> UpdateAsync(int id, UpdateVeilingProductDto dto);
    Task<Result<bool>> DeleteAsync(int id);

    // EXTRA voor dashboard (case)
    Task<Result<VeilingProductDto>> SetActiveAsync(int id);
    Task<Result<VeilingProductDto>> MarkAsSoldAsync(int id, int koperId);
}