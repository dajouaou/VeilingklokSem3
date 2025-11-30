using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public interface IVeilingmeesterDashboardService
{
    // READ
    Task<VeilingDetailsDto?> GetVeilingDetailsAsync(int veilingId);
    Task<CurrentLotDto?> GetCurrentLotAsync(int veilingId);
    Task<List<QueueGroupDto>> GetQueueAsync(int veilingId);
    Task<List<QueueGroupDto>> GetQueueAllAsync(int veilingId);
    Task<List<BidDto>> GetBidsAsync(int veilingId, int? veilingProductId = null);
    Task<List<AuditDto>> GetAuditAsync(int veilingId);

    // COMMANDS
    Task<int> StartAuctionAsync(StartAuctionRequestDto dto); // return veilingId (CreatedAtAction)
    Task PauseAuctionAsync(int veilingId);
    Task ResumeAuctionAsync(int veilingId);
    Task StopAuctionAsync(int veilingId);

    Task AddQueueItemAsync(int veilingId, AddQueueItemDto dto);
    Task ReorderQueueAsync(int veilingId, ReorderQueueDto dto);

    Task<BidDto> PlaceBidAsync(int veilingId, PlaceBidDto dto);
}