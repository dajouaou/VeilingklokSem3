// Veilingklok/Features/VM/Services/IVMService.cs
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VM.Dtos;

namespace Veilingklok.Features.VM.Services;

public interface IVMService
{
    Task<Result<VMVeilingDashboardDto>> GetDashboardAsync(int veilingId);
    Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId);
    Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId);
    Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId);
}
