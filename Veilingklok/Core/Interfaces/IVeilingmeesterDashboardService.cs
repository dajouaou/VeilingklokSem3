using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Auction;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Audit;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Bids;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Lot;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Queue;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public interface IVeilingmeesterDashboardService
{
    // READ
    Task<VeilingDetailsDto?> GetVeilingDetailsAsync(int veilingId);
    Task<CurrentLotDto?> GetCurrentLotAsync(int veilingId);
    Task<List<QueueItemDto>> GetQueueAsync(int veilingId);        // alleen queued
    Task<List<QueueItemDto>> GetQueueAllAsync(int veilingId);     // alle items
    Task<List<BidDto>> GetBidsAsync(int veilingId, int? veilingProductId = null);
    Task<List<AuditEntryDto>> GetAuditEntriesAsync(int veilingId);

    // COMMANDS
    Task<int> StartAuctionAsync(StartAuctionRequestDto dto);
    Task PauseAuctionAsync(int veilingId);
    Task ResumeAuctionAsync(int veilingId);
    Task StopAuctionAsync(int veilingId);

    Task AddQueueItemAsync(int veilingId, AddQueueItemDto dto);
    Task ReorderQueueAsync(int veilingId, ReorderQueueDto dto);

    Task<Result<BidDto>> PlaceBidAsync(int veilingId, PlaceBidDto dto);
}