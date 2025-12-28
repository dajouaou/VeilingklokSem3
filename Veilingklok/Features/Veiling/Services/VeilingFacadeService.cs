using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Features.VM.Services;

namespace Veilingklok.Features.Veiling.Services;

public sealed class VeilingFacadeService : IVeilingFacadeService
{
    private readonly IVeilingPublicService _public;
    private readonly IBiddingService _bidding;
    private readonly IAuctionClockService _clock;
    private readonly IVeilingPlanningService _planning;
    private readonly IVMService _vm;

    public VeilingFacadeService(
        IVeilingPublicService @public,
        IBiddingService bidding,
        IAuctionClockService clock,
        IVeilingPlanningService planning,
        IVMService vm)
    {
        _public = @public;
        _bidding = bidding;
        _clock = clock;
        _planning = planning;
        _vm = vm;
    }

    public Task<Result<List<string>>> GetBeschikbareLeverdagenAsync()
        => _public.GetBeschikbareLeverdagenAsync();

    public Task<Result<List<string>>> GetBeschikbareVeildagenAsync()
        => _public.GetBeschikbareLeverdagenAsync();

    public Task<Result<PublicVeilingDto>> LoadPublicAsync(int veilingId)
        => _public.LoadPublicAsync(veilingId);

    public Task<Result<BidResultDto>> PlaceBidAsync(int veilingId, int koperId)
        => _bidding.PlaceBidAsync(veilingId, koperId, null);

    public Task TickAsync(CancellationToken ct)
        => _clock.TickAsync(ct);

    public Task<Result<int>> CreateVeilingFromLeverdatumAsync(CreateVeilingFromLeverdatumDto dto, int actorGebruikerId)
        => _planning.CreateVeilingFromLeverdatumAsync(dto, actorGebruikerId);

    public Task<Result<VeilingDetailsDto>> GetDetailsAsync(int veilingId)
        => _planning.GetDetailsAsync(veilingId);

    public Task<Result<List<string>>> GetVeildagenAsync()
        => _planning.GetVeildagenAsync();

    public Task<Result<List<VeilingPlanningAanmeldingDto>>> GetAanmeldingenAsync(string leverdatum)
        => _planning.GetAanmeldingenAsync(leverdatum);

    public Task<Result<int>> PlanVeilingAsync(PlanVeilingRequestDto dto, int actorGebruikerId)
        => _planning.PlanVeilingAsync(dto, actorGebruikerId);

    public Task<Result<List<GeplandeVeilingListItemDto>>> GetGeplandeAsync()
        => _planning.GetGeplandeAsync();

    public Task<Result<GeplandeVeilingListItemDto?>> GetVolgendeGeplandeAsync()
        => _planning.GetVolgendeGeplandeAsync();

    public Task<Result<VMVeilingDashboardDto>> GetDashboardAsync(int veilingId)
        => _vm.GetDashboardAsync(veilingId);

    public Task<Result<VMActiveVeilingDto?>> GetActieveVeilingAsync()
        => _vm.GetActieveVeilingAsync();

    public Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId, int actorGebruikerId)
        => _vm.StartVeilingAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> PauseVeilingAsync(int veilingId, int actorGebruikerId)
        => _vm.PauseVeilingAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> ResumeVeilingAsync(int veilingId, int actorGebruikerId)
        => _vm.ResumeVeilingAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> StopVeilingAsync(int veilingId, int actorGebruikerId)
        => _vm.StopVeilingAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId, int actorGebruikerId)
        => _vm.ActivateNextProductAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId, int actorGebruikerId)
        => _vm.CloseCurrentProductAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> ResetVeilingAsync(int veilingId, int actorGebruikerId)
        => _vm.ResetVeilingAsync(veilingId, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> ReorderQueueAsync(int veilingId, VMReorderQueueRequest request, int actorGebruikerId)
        => _vm.ReorderQueueAsync(veilingId, request, actorGebruikerId);

    public Task<Result<VMVeilingDashboardDto>> SkipProductAsync(int veilingId, int veilingProductId, int actorGebruikerId)
        => _vm.SkipProductAsync(veilingId, veilingProductId, actorGebruikerId);
}
