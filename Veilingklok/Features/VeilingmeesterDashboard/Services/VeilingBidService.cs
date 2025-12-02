using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public sealed class VeilingBidService : IVeilingBidService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuctionEventDispatcher _events;

    public VeilingBidService(MyContext db, IMapper mapper, IAuctionEventDispatcher events)
    {
        _db = db;
        _mapper = mapper;
        _events = events;
    }

    public async Task<Result<BidDto>> PlaceMeesterBidAsync(int veilingId, int lotId, decimal amount, int actorGebruikerId)
    {
        var lot = await _db.VeilingProducten
            .Include(vp => vp.Veiling)
            .FirstOrDefaultAsync(vp => vp.Id == lotId && vp.VeilingId == veilingId);

        if (lot is null)
            return Result.Fail<BidDto>("Lot niet gevonden.");

        if (lot.Status != VeilingProductStatus.Running)
            return Result.Fail<BidDto>("Alleen actieve loten kunnen een bod ontvangen.");

        var entity = new Bid
        {
            VeilingId = veilingId,
            VeilingProductId = lotId,
            PlacedByGebruikerId = actorGebruikerId,
            KoperId = null,
            Amount = amount,
            Source = BidSource.Auctioneer,
            PlacedAtUtc = DateTime.UtcNow
        };

        _db.Bids.Add(entity);
        lot.LaatsteBodBedrag = amount;
        lot.BiedCount++;

        await _db.SaveChangesAsync();

        await _events.BidPlaced(new()
        {
            VeilingId = veilingId,
            VeilingProductId = lotId,
            Amount = amount,
            PlacedAtUtc = entity.PlacedAtUtc
        });

        return Result.Success(_mapper.Map<BidDto>(entity));
    }
}
