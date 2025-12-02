using Veilingklok.Core.Shared;
using Veilingklok.Features.Veilingmeester.Dtos;

namespace Veilingklok.Core.Interfaces;

public interface IVeilingmeesterService
{
    Task<Result<VeilingmeesterDto>> CreateAsync(CreateVeilingmeesterDto dto);
    Task<Result<VeilingmeesterDto>> GetByIdAsync(int id);
    Task<Result<List<VeilingmeesterDto>>> GetAllAsync();
    Task<Result<VeilingmeesterDto>> UpdateAsync(int id, UpdateVeilingmeesterDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}