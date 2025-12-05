using Veilingklok.Core.Entities;
using Veilingklok.Core.Shared;

namespace Veilingklok.Features.VM.Services;

public interface IVMService
{
    Task<Result<Core.Entities.Veiling>> StartVeilingAsync(int veilingId);
    Task<Result<VeilingProduct>> ActivateNextProductAsync(int veilingId);
    Task<Result<bool>> CloseCurrentProductAsync(int veilingId);
    Task<Result<Core.Entities.Veiling>> GetDashboardStateAsync(int veilingId);
}