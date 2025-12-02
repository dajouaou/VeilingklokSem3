using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services;

public sealed class BidService : IBidService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuctionEventDispatcher _events;

    public BidService(MyContext db, IMapper mapper, IAuctionEventDispatcher events)
    {
        _db = db;
        _mapper = mapper;
        _events = events;
    }

    // ============================================================
    // 1. Plaats bod
    // ============================================================
    public async Task<Result<BidDto>> PlaceBidAsync(int gebruikerId, int veilingId, PlaceBidRequestDto dto)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.CurrentVeilingProduct)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling is null)
            return Result.Fail<BidDto>("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Running)
            return Result.Fail<BidDto>("Veiling is niet actief.");

        if (veiling.CurrentVeilingProduct is null)
            return Result.Fail<BidDto>("Geen actief lot geselecteerd.");

        if (veiling.CurrentVeilingProduct.Id != dto.VeilingProductId)
            return Result.Fail<BidDto>("Dit lot is niet actief.");

        var koper = await _db.Kopers.FirstOrDefaultAsync(k => k.GebruikerId == gebruikerId);
        if (koper is null)
            return Result.Fail<BidDto>("Alleen kopers mogen bieden.");

        var lastBid = await _db.Bids
            .Where(b => b.VeilingProductId == dto.VeilingProductId)
            .OrderByDescending(b => b.PlacedAtUtc)
            .FirstOrDefaultAsync();

        if (lastBid != null && dto.Amount <= lastBid.Amount)
            return Result.Fail<BidDto>("Bod moet hoger zijn dan het laatste bod.");

        var entity = new Bid
        {
            VeilingId = veilingId,
            VeilingProductId = dto.VeilingProductId,
            PlacedByGebruikerId = gebruikerId,
            KoperId = koper.Id,
            Amount = dto.Amount,
            Source = BidSource.Buyer,
            PlacedAtUtc = DateTime.UtcNow
        };

        _db.Bids.Add(entity);
        await _db.SaveChangesAsync();

        var publicDto = _mapper.Map<PublicBidDto>(entity);
        await _events.BidPlacedAsync(publicDto);

        return Result.Success(_mapper.Map<BidDto>(entity));
    }

    // ============================================================
    // 2. Biedingen per veiling
    // ============================================================
    public async Task<Result<List<BidListItemDto>>> GetForVeilingAsync(int veilingId)
    {
        var list = await _db.Bids
            .Where(b => b.VeilingId == veilingId)
            .OrderByDescending(b => b.PlacedAtUtc)
            .ProjectTo<BidListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    // ============================================================
    // 3. Biedingen per lot
    // ============================================================
    public async Task<Result<List<BidListItemDto>>> GetForVeilingProductAsync(int veilingId, int veilingProductId)
    {
        var list = await _db.Bids
            .Where(b => b.VeilingId == veilingId && b.VeilingProductId == veilingProductId)
            .OrderByDescending(b => b.PlacedAtUtc)
            .ProjectTo<BidListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }
}
