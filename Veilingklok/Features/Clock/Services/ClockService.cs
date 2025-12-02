using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Realtime.Dtos;   // <-- juiste DTO's
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Clock.Services;

public sealed class ClockService : IClockService
{
    private readonly MyContext _db;
    private readonly IAuctionEventDispatcher _events;

    private static readonly Dictionary<int, CancellationTokenSource> Timers = new();

    public ClockService(MyContext db, IAuctionEventDispatcher events)
    {
        _db = db;
        _events = events;
    }

    public async Task<Result<bool>> StartClockAsync(int veilingProductId)
    {
        var vp = await _db.VeilingProducten
            .FirstOrDefaultAsync(x => x.Id == veilingProductId);

        if (vp is null)
            return Result.Fail<bool>("Veilingproduct niet gevonden.");

        if (vp.Status != VeilingProductStatus.Running)
            return Result.Fail<bool>("Veilingproduct moet Running zijn.");

        if (Timers.TryGetValue(vp.Id, out var existing))
        {
            existing.Cancel();
            Timers.Remove(vp.Id);
        }

        var cts = new CancellationTokenSource();
        Timers[vp.Id] = cts;

        _ = RunClockAsync(vp.Id, cts.Token);

        return Result.Success(true);
    }

    private async Task RunClockAsync(int veilingProductId, CancellationToken token)
    {
        var vp = await _db.VeilingProducten
            .FirstAsync(x => x.Id == veilingProductId);

        var timeLeft = vp.ClockDurationMs;

        while (timeLeft > 0 && !token.IsCancellationRequested)
        {
            await Task.Delay(vp.ClockTickMs, token);

            vp.HuidigePrijs -= vp.PriceDropPerTick;
            timeLeft -= vp.ClockTickMs;

            if (vp.HuidigePrijs <= vp.MinimumPrijs)
            {
                vp.HuidigePrijs = vp.MinimumPrijs;
                timeLeft = 0;
            }

            await _db.SaveChangesAsync();

            await _events.ClockTickAsync(new ClockTickDto
            {
                VeilingId = vp.VeilingId,
                VeilingProductId = vp.Id,
                TimeLeftMs = timeLeft,
                CurrentPrice = vp.HuidigePrijs
            });
        }

        if (!token.IsCancellationRequested)
        {
            vp.Status = VeilingProductStatus.Unsold;
            vp.ClosedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _events.CurrentLotChangedAsync(new CurrentLotChangedDto
            {
                VeilingId = vp.VeilingId,
                VeilingProductId = vp.Id,
                NewStatus = VeilingProductStatus.Unsold.ToString()
            });
        }

        Timers.Remove(veilingProductId);
    }
}
