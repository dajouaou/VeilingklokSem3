using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.Veiling.Services;

public interface IVeilingPlanningService
{
    Task<Result<int>> CreateVeilingFromLeverdatumAsync(CreateVeilingFromLeverdatumDto dto, int actorGebruikerId);
    Task<Result<List<string>>> GetVeildagenAsync();
    Task<Result<List<VeilingPlanningAanmeldingDto>>> GetAanmeldingenAsync(string leverdatum);
    Task<Result<int>> PlanVeilingAsync(PlanVeilingRequestDto dto, int actorGebruikerId);
    Task<Result<List<GeplandeVeilingListItemDto>>> GetGeplandeAsync();
    Task<Result<GeplandeVeilingListItemDto?>> GetVolgendeGeplandeAsync();
    Task<Result<VeilingDetailsDto>> GetDetailsAsync(int veilingId);
}
