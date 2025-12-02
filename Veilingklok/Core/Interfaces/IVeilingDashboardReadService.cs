using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Auction;

namespace Veilingklok.Core.Interfaces;

public interface IVeilingDashboardReadService
{
    Task<VeilingDetailsDto?> GetDetailsAsync(int veilingId);
    Task<CurrentLotDto?> GetCurrentLotAsync(int veilingId);
}