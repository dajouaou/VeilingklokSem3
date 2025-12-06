using Veilingklok.Core.Entities;
using Veilingklok.Core.Shared;

namespace Veilingklok.Features.VM.Services;

public interface IVMService
{
    //StartVeilingAsync
    Task<Result<Core.Entities.Veiling>> StartVeilingAsync(int veilingId);
    
    //ActivateNextProductAsync
    Task<Result<VeilingProduct>> ActivateNextProductAsync(int veilingId);
    
    //CloseCurrentProductAsync
    Task<Result<bool>> CloseCurrentProductAsync(int veilingId);
    
    //GetDashboardStateAsync
    Task<Result<Core.Entities.Veiling>> GetDashboardStateAsync(int veilingId);
}