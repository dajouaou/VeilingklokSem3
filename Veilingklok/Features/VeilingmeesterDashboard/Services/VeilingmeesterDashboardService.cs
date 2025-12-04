using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR;
using Veilingklok.Infrastructure.SignalR.Events;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public sealed class VeilingmeesterDashboardService : IVeilingmeesterDashboardService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuctionEventDispatcher _events;

    public VeilingmeesterDashboardService(MyContext db, IMapper mapper, IAuctionEventDispatcher events)
    {
        _db = db;
        _mapper = mapper;
        _events = events;
    }

    // READ: details (no tracking)
    public async Task<VeilingDetailsDto?> GetVeilingDetailsAsync(int veilingId)
    {
        var v = await _db.Veilingen
            .AsNoTracking()
            .Include(x => x.CurrentVeilingProduct)!.ThenInclude(vp => vp!.Product)!.ThenInclude(p => p!.Aanvoerder)
            .FirstOrDefaultAsync(x => x.Id == veilingId);

        if (v == null) return null;

        var dto = _mapper.Map<VeilingDetailsDto>(v);
        dto.CurrentVeilingProductId = v.CurrentVeilingProductId;
        dto.CurrentProductNaam = v.CurrentVeilingProduct?.Product?.Naam;
        dto.CurrentPrijs = v.CurrentVeilingProduct?.HuidigePrijs;
        return dto;
    }

    // READ: current lot (no Select+Include mix)
    public async Task<CurrentLotDto?> GetCurrentLotAsync(int veilingId)
    {
        var veiling = await _db.Veilingen
            .AsNoTracking()
            .Include(v => v.CurrentVeilingProduct)!.ThenInclude(vp => vp!.Product)!.ThenInclude(p => p!.Aanvoerder)
            .Include(v => v.CurrentVeilingProduct)!.ThenInclude(vp => vp!.Bids)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        var lot = veiling?.CurrentVeilingProduct;
        if (lot == null) return null;

        var dto = _mapper.Map<CurrentLotDto>(lot);

        dto.LastBid = lot.Bids
            .OrderByDescending(b => b.PlacedAtUtc)
            .FirstOrDefault()
            ?.Amount;

        dto.BidCount = lot.Bids.Count;
        return dto;
    }

    // READ: queue (queued only)
    public Task<List<QueueGroupDto>> GetQueueAsync(int veilingId)
        => GetQueueInternalAsync(veilingId, onlyQueued: true);

    // READ: queue (all statuses)
    public Task<List<QueueGroupDto>> GetQueueAllAsync(int veilingId)
        => GetQueueInternalAsync(veilingId, onlyQueued: false);

    private async Task<List<QueueGroupDto>> GetQueueInternalAsync(int veilingId, bool onlyQueued)
    {
        var q = _db.VeilingProducten
            .AsNoTracking()
            .Where(x => x.VeilingId == veilingId)
            .Include(x => x.Product)!.ThenInclude(p => p!.Aanvoerder)
            .AsQueryable();

        if (onlyQueued)
            q = q.Where(x => x.Status == VeilingProductStatus.Queued);

        var data = await q.OrderBy(x => x.Volgorde).ToListAsync();

        return data
            .GroupBy(x => x.Product!.Aanvoerder!)
            .Select(g => new QueueGroupDto
            {
                AanvoerderId = g.Key.Id,
                AanvoerderNaam = g.Key.Naam,
                Items = _mapper.Map<List<QueueItemDto>>(g.OrderBy(x => x.Volgorde).ToList())
            })
            .ToList();
    }

    // READ: bids (default = current lot)
    public async Task<List<BidDto>> GetBidsAsync(int veilingId, int? veilingProductId = null)
    {
        if (veilingProductId is null)
        {
            veilingProductId = await _db.Veilingen
                .AsNoTracking()
                .Where(v => v.Id == veilingId)
                .Select(v => v.CurrentVeilingProductId)
                .FirstOrDefaultAsync();
        }

        if (veilingProductId is null) return new List<BidDto>();

        var list = await _db.Biedingen
            .AsNoTracking()
            .Where(b => b.VeilingId == veilingId && b.VeilingProductId == veilingProductId)
            .Include(b => b.Koper)
            .OrderByDescending(b => b.PlacedAtUtc)
            .ToListAsync();

        return _mapper.Map<List<BidDto>>(list);
    }

    // READ: audit (no tracking)
    public async Task<List<AuditDto>> GetAuditAsync(int veilingId)
    {
        var list = await _db.AuditEntries
            .AsNoTracking()
            .Where(a => a.VeilingId == veilingId)
            .Include(a => a.ActorGebruiker)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync();

        return _mapper.Map<List<AuditDto>>(list);
    }

    // COMMAND: start auction (return id + no silent fail)
    public async Task<int> StartAuctionAsync(StartAuctionRequestDto dto)
    {
        if (dto.ProductIds is null || dto.ProductIds.Count == 0)
            throw new ArgumentException("ProductIds mag niet leeg zijn.", nameof(dto.ProductIds));

        var v = new Veilingklok.Core.Entities.Veiling
        {
            Status = VeilingStatus.Scheduled,
            StartTijdUtc = dto.StartTijdUtc,
            EindTijdUtc = dto.EindTijdUtc
        };

        _db.Veilingen.Add(v);
        await _db.SaveChangesAsync();

        var i = 1;
        foreach (var productId in dto.ProductIds.Distinct())
        {
            _db.VeilingProducten.Add(new VeilingProduct
            {
                VeilingId = v.Id,
                ProductId = productId,
                Status = VeilingProductStatus.Queued,
                StartPrijs = 0,
                HuidigePrijs = 0,
                Volgorde = i++,
                Hoeveelheid = 1
            });
        }

        await _db.SaveChangesAsync();

        // realtime via dispatcher (1 plek)
        await _events.Publish(new AuctionStatusChangedEvent(v.Id, v.Status.ToString()));
        await _events.Publish(new QueueUpdatedEvent(v.Id));

        return v.Id;
    }

    // COMMAND: status changes
    public Task PauseAuctionAsync(int veilingId) => SetStatusAsync(veilingId, VeilingStatus.Paused);
    public Task ResumeAuctionAsync(int veilingId) => SetStatusAsync(veilingId, VeilingStatus.Running);
    public Task StopAuctionAsync(int veilingId) => SetStatusAsync(veilingId, VeilingStatus.Finished);

    private async Task SetStatusAsync(int veilingId, VeilingStatus status)
    {
        var v = await _db.Veilingen.FindAsync(veilingId);
        if (v == null) return;

        v.Status = status;
        await _db.SaveChangesAsync();

        await _events.Publish(new AuctionStatusChangedEvent(veilingId, v.Status.ToString()));
    }

    // COMMAND: add queue item (auto volgorde)
    public async Task AddQueueItemAsync(int veilingId, AddQueueItemDto dto)
    {
        var volgorde = dto.Volgorde;

        if (volgorde <= 0)
        {
            var last = await _db.VeilingProducten
                .Where(x => x.VeilingId == veilingId)
                .MaxAsync(x => (int?)x.Volgorde) ?? 0;

            volgorde = last + 1;
        }

        _db.VeilingProducten.Add(new VeilingProduct
        {
            VeilingId = veilingId,
            ProductId = dto.ProductId,
            Volgorde = volgorde,
            Hoeveelheid = dto.Hoeveelheid,
            StartPrijs = dto.StartPrijs,
            HuidigePrijs = dto.StartPrijs,
            Status = VeilingProductStatus.Queued
        });

        await _db.SaveChangesAsync();

        await _events.Publish(new QueueUpdatedEvent(veilingId));
    }

    // COMMAND: reorder (no N+1)
    public async Task ReorderQueueAsync(int veilingId, ReorderQueueDto dto)
    {
        var ids = dto.Items.Select(x => x.VeilingProductId).Distinct().ToList();

        var lots = await _db.VeilingProducten
            .Where(x => x.VeilingId == veilingId && ids.Contains(x.Id))
            .ToListAsync();

        var map = lots.ToDictionary(x => x.Id);

        foreach (var item in dto.Items)
            if (map.TryGetValue(item.VeilingProductId, out var lot))
                lot.Volgorde = item.NewPosition;

        await _db.SaveChangesAsync();

        await _events.Publish(new QueueUpdatedEvent(veilingId));
    }

    // COMMAND: place bid
    public async Task<BidDto> PlaceBidAsync(int veilingId, PlaceBidDto dto)
    {
        var koperId = dto.Source == BidSource.Auctioneer ? null : dto.KoperId;

        var bid = new Bid
        {
            VeilingId = veilingId,
            VeilingProductId = dto.VeilingProductId,
            PlacedByGebruikerId = dto.GebruikerId,
            KoperId = koperId,
            Amount = dto.Amount,
            Source = dto.Source,
            PlacedAtUtc = DateTime.UtcNow
        };

        _db.Biedingen.Add(bid);

        var lot = await _db.VeilingProducten
            .FirstOrDefaultAsync(x => x.Id == dto.VeilingProductId && x.VeilingId == veilingId);

        if (lot != null)
            lot.HuidigePrijs = dto.Amount;

        await _db.SaveChangesAsync();

        // kopernaam voor response
        await _db.Entry(bid).Reference(b => b.Koper).LoadAsync();

        var mapped = _mapper.Map<BidDto>(bid);

        await _events.Publish(new BidPlacedEvent(veilingId, mapped));
        return mapped;
    }
}
