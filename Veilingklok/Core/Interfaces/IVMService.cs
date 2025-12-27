// Veilingklok/Features/VM/Services/IVMService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VM.Services;

public interface IVMService
{
    Task<Result<VMVeilingDashboardDto>> GetDashboardAsync(int veilingId);
    Task<Result<VMActiveVeilingDto?>> GetActieveVeilingAsync();

    Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId, int actorGebruikerId);
    Task<Result<VMVeilingDashboardDto>> PauseVeilingAsync(int veilingId, int actorGebruikerId);
    Task<Result<VMVeilingDashboardDto>> ResumeVeilingAsync(int veilingId, int actorGebruikerId);
    Task<Result<VMVeilingDashboardDto>> StopVeilingAsync(int veilingId, int actorGebruikerId);

    Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId, int actorGebruikerId);
    Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId, int actorGebruikerId);

    Task<Result<VMVeilingDashboardDto>> ResetVeilingAsync(int veilingId, int actorGebruikerId);
    Task<Result<VMVeilingDashboardDto>> ReorderQueueAsync(int veilingId, VMReorderQueueRequest request, int actorGebruikerId);
    Task<Result<VMVeilingDashboardDto>> SkipProductAsync(int veilingId, int veilingProductId, int actorGebruikerId);

    Task<Result<List<string>>> GetVeildagenAsync();
    Task<Result<List<VeilingPlanningAanmeldingDto>>> GetAanmeldingenAsync(string leverdatum);
    Task<Result<int>> PlanVeilingAsync(PlanVeilingRequestDto dto, int actorGebruikerId);
    Task<Result<List<GeplandeVeilingListItemDto>>> GetGeplandeAsync();
    Task<Result<GeplandeVeilingListItemDto?>> GetVolgendeGeplandeAsync();
}