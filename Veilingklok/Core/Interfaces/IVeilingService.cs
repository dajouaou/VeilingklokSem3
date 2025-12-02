using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Core.Interfaces;

public interface IVeilingService
{
    Task<Result<VeilingDto>> CreateAsync(CreateVeilingDto dto);
    Task<Result<VeilingDto>> GetByIdAsync(int id);
    Task<Result<List<VeilingDto>>> GetAllAsync();
    Task<Result<VeilingDto>> UpdateAsync(int id, UpdateVeilingDto dto);
    Task<Result<bool>> DeleteAsync(int id);

    Task<Result<VeilingDto>> StartVeilingAsync(int id, StartVeilingDto dto);
    Task<Result<VeilingDto>> SetCurrentLotAsync(int id, ChangeCurrentLotDto dto);
    Task<Result<VeilingDto>> StopVeilingAsync(int id);
}