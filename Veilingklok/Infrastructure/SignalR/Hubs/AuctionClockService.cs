// Veilingklok/Features/Veiling/Services/AuctionClockService.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Services;

public sealed class AuctionClockService : IAuctionClockService
{
    private const int DefaultDurationSeconds = 20;

    private readonly MyContext _db;
    private readonly IVeilingBroadcastService _broadcast;

    public AuctionClockService(MyContext db, IVeilingBroadcastService broadcast)
    {
        _db = db;
        _broadcast = broadcast;
    }

    public async Task TickAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var runningVeilingIds = await _db.Veilingen
            .AsNoTracking()
            .Where(v => v.Status == VeilingStatus.Running)
            .Select(v => v.Id)
            .ToListAsync(ct);

        foreach (var veilingId in runningVeilingIds)
        {
            ct.ThrowIfCancellationRequested();

            var veiling = await _db.Veilingen
                .Include(v => v.VeilingProducten)
                    .ThenInclude(vp => vp.Aanmelding)
                .FirstOrDefaultAsync(v => v.Id == veilingId, ct);

            if (veiling == null || veiling.CurrentVeilingProductId == null)
            {
                await _broadcast.StuurTick(veilingId, new TickDto { VeilingId = veilingId, VeilingProductId = null, CurrentPrice = 0m, TimeLeftMs = 0 });
                continue;
            }

            var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId.Value);

            if (current == null || current.Status != VeilingProductStatus.Active || current.ClosedAtUtc != null)
            {
                await _broadcast.StuurTick(veilingId, new TickDto { VeilingId = veilingId, VeilingProductId = null, CurrentPrice = 0m, TimeLeftMs = 0 });
                continue;
            }

            current.ActivatedAtUtc ??= now;

            var min = current.MinimumPrijs > 0m ? current.MinimumPrijs : current.Aanmelding?.MinimumPrijs ?? current.StartPrijs;
            var total = current.DurationSeconds > 0 ? current.DurationSeconds : DefaultDurationSeconds;

            var elapsed = now - current.ActivatedAtUtc.Value;
            var t = Math.Clamp(elapsed.TotalSeconds / total, 0.0, 1.0);

            var newPrice = current.StartPrijs - (decimal)t * (current.StartPrijs - min);
            if (newPrice < min) newPrice = min;

            current.HuidigePrijs = newPrice;

            var timeLeftMs = (int)Math.Max(0, (total - elapsed.TotalSeconds) * 1000);

            await _db.SaveChangesAsync(ct);

            await _broadcast.StuurTick(veilingId, new TickDto
            {
                VeilingId = veilingId,
                VeilingProductId = current.Id,
                CurrentPrice = current.HuidigePrijs,
                TimeLeftMs = timeLeftMs
            });

            if (timeLeftMs > 0 && current.HuidigePrijs > min)
                continue;

            current.ClosedAtUtc = now;
            current.Status = VeilingProductStatus.Skipped;
            await _db.SaveChangesAsync(ct);

            var next = veiling.VeilingProducten
                .Where(p => p.Status == VeilingProductStatus.Queued)
                .OrderBy(p => p.Volgorde)
                .ThenBy(p => p.Id)
                .FirstOrDefault();

            if (next != null)
            {
                next.Status = VeilingProductStatus.Active;
                next.ActivatedAtUtc = now;
                veiling.CurrentVeilingProductId = next.Id;
            }
            else
            {
                veiling.CurrentVeilingProductId = null;
                veiling.Status = VeilingStatus.Finished;
            }

            await _db.SaveChangesAsync(ct);

            await _broadcast.StuurWachtrij(veilingId, veiling.VeilingProducten
                .Where(p => p.Status == VeilingProductStatus.Queued)
                .OrderBy(p => p.Volgorde)
                .ThenBy(p => p.Id)
                .Select(p => new WachtrijItemDto
                {
                    VeilingProductId = p.Id,
                    Volgorde = p.Volgorde,
                    Soort = p.Aanmelding?.Soort ?? string.Empty,
                    FotoUrl = p.Aanmelding?.FotoUrl,
                    StartPrijs = p.StartPrijs,
                    Hoeveelheid = p.Aanmelding?.Hoeveelheid ?? p.Hoeveelheid
                })
                .ToList());

            if (next != null)
            {
                await _broadcast.StuurHuidigProduct(veilingId, new HuidigProductDto
                {
                    VeilingProductId = next.Id,
                    Soort = next.Aanmelding?.Soort ?? string.Empty,
                    FotoUrl = next.Aanmelding?.FotoUrl,
                    StartPrijs = next.StartPrijs,
                    HuidigePrijs = next.HuidigePrijs,
                    Hoeveelheid = next.Aanmelding?.Hoeveelheid ?? next.Hoeveelheid,
                    IsActief = true,
                    IsVerkocht = false
                });
            }
        }
    }
}
