using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Core.Interfaces
{
    public interface IPlanningService
    {
        Task<List<PlanProductDto>> GetBeschikbareProductenAsync();
        Task<VeilingDto> StartVeilingMetPlanningAsync(StartVeilingRequestDto dto);
    }
}
