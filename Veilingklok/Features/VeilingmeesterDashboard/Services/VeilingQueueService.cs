using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR;
using Veilingklok.Infrastructure.SignalR.Events;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public sealed class VeilingQueueService : IVeilingQueueService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuctionEventDispatcher _events;

    public VeilingQueueService(MyContext db, IMapper mapper, IAuctionEventDispatcher events)
    {
        _db = db;
        _mapper = mapper;
        _events = events;
    }

    public async Task<List<QueueGroupDto>> GetQueueAsync(int veilingId, bool onlyQueued)
    {
        var q = _db.VeilingProducten
            .AsNoTracking()
            .Where(x => x.VeilingId == veilingId)
            .Include(x => x.Product)!.ThenInclude(p => p!.Aanvoerder)
            .AsQueryable();

        if (onlyQueued)
            q = q.Where(x => x.Status == VeilingProductStatus.InQueue);

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

    public async Task<Result> AddQueueItemAsync(int veilingId, AddQueueItemDto dto, int actorGebruikerId)
    {
        if (!await _db.Veilingen.AnyAsync(v => v.Id == veilingId))
            return Result.Fail("Veiling niet gevonden.");

        var volgorde = dto.Volgorde;

        if (volgorde <= 0)
        {
            var last = await _db.VeilingProducten
                .Where(x => x.VeilingId == veilingId)
                .MaxAsync(x => (int?)x.Volgorde) ?? 0;

            volgorde = last + 1;
        }

        var lot = new VeilingProduct
        {
            VeilingId = veilingId,
            ProductId = dto.ProductId,
            Volgorde = volgorde,
            Hoeveelheid = dto.Hoeveelheid,
            StartPrijs = dto.StartPrijs,
            HuidigePrijs = dto.StartPrijs,
            Status = VeilingProductStatus.InQueue
        };

        _db.VeilingProducten.Add(lot);
        await _db.SaveChangesAsync();

        await _events.Publish(new QueueUpdatedEvent(veilingId));

        return Result.Success();
    }

    public async Task<Result> ReorderQueueAsync(int veilingId, ReorderQueueDto dto, int actorGebruikerId)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return Result.Fail("Items mag niet leeg zijn.");

        var ids = dto.Items.Select(x => x.VeilingProductId).Distinct().ToList();

        var lots = await _db.VeilingProducten
            .Where(x => x.VeilingId == veilingId && ids.Contains(x.Id))
            .ToListAsync();

        if (lots.Count == 0)
            return Result.Fail("Geen veilingproducten gevonden.");

        foreach (var item in dto.Items)
        {
            var lot = lots.FirstOrDefault(x => x.Id == item.VeilingProductId);
            if (lot != null)
                lot.Volgorde = item.NewPosition;
        }

        await _db.SaveChangesAsync();
        await _events.Publish(new QueueUpdatedEvent(veilingId));

        return Result.Success();
    }
}
