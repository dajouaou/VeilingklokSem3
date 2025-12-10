using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Core.Interfaces
{
    public interface IPlanningService
    {
        Task<List<PlanProductDto>> GetProductenPerVeildatumAsync(DateTime? veildatum);
        Task<List<VeilingPlanningDto>> GetGeplandeVeilingenAsync();
        Task<VeilingPlanningDto> CreateVeilingAsync(VeilingCreateDto dto);
        Task<VeilingPlanningDto> UpdateVeilingAsync(int id, VeilingCreateDto dto);
        Task DeleteVeilingAsync(int id);
        Task<VeilingPlanningDto> StartGeplandeVeilingAsync(int id);
    }
}
