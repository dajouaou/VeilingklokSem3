using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VM.Dtos;

namespace Veilingklok.Features.Veiling.Services;

public interface IVeilingFacadeService
{
    Task<Result<List<string>>> GetBeschikbareLeverdagenAsync();
    Task<Result<List<string>>> GetBeschikbareVeildagenAsync();

    Task<Result<int>> CreateVeilingFromLeverdatumAsync(CreateVeilingFromLeverdatumDto dto, int actorGebruikerId);
    Task<Result<VeilingDetailsDto>> GetDetailsAsync(int veilingId);

    Task<Result<PublicVeilingDto>> LoadPublicAsync(int veilingId);
    Task<Result<BidResultDto>> PlaceBidAsync(int veilingId, int koperId);

    Task TickAsync(CancellationToken ct);

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
