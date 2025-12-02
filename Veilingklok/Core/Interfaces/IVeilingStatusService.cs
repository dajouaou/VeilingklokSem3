using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Auction;

namespace Veilingklok.Core.Interfaces;

public interface IVeilingStatusService
{
    Task<Result<int>> StartAuctionAsync(StartAuctionRequestDto dto);
    Task<Result> PauseAsync(int veilingId);
    Task<Result> ResumeAsync(int veilingId);
    Task<Result> StopAsync(int veilingId);
}