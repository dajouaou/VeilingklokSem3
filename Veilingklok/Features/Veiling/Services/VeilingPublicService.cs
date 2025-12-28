using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services;

public sealed class VeilingPublicService : IVeilingPublicService
{
    private const int DefaultDurationSeconds = 20;
    private readonly MyContext _db;

    public VeilingPublicService(MyContext db)
    {
        _db = db;
    }

    public async Task<Result<List<string>>> GetBeschikbareLeverdagenAsync()
    {
        var dagen = await _db.Aanmeldingen
            .AsNoTracking()
            .Select(a => a.LeverDatum.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync();

        return Result<List<string>>.Ok(dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList());
    }

    public async Task<Result<PublicVeilingDto>> LoadPublicAsync(int veilingId)
    {
        var veiling = await _db.Veilingen
            .AsNoTracking()
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Aanmelding)
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Bids)
                    .ThenInclude(b => b.Koper)
                    .ThenInclude(k => k!.Gebruiker)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<PublicVeilingDto>.Fail("Veiling niet gevonden.");

        var current = veiling.CurrentVeilingProductId is int currentId
            ? veiling.VeilingProducten.FirstOrDefault(p => p.Id == currentId && p.Status == VeilingProductStatus.Active)
            : null;

        var queue = veiling.VeilingProducten
            .Where(p => p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .ThenBy(p => p.Id)
            .ToList();

        var bids = current?.Bids
                .OrderByDescending(b => b.PlacedAtUtc)
                .ToList()
            ?? new List<Bid>();

        var timeLeftMs = 0;

        if (current?.ActivatedAtUtc != null)
        {
            var total = current.DurationSeconds > 0 ? current.DurationSeconds : DefaultDurationSeconds;
            var elapsed = System.DateTime.UtcNow - current.ActivatedAtUtc.Value;
            timeLeftMs = System.Math.Max(0, (int)((total - elapsed.TotalSeconds) * 1000));
        }

        return Result<PublicVeilingDto>.Ok(new PublicVeilingDto
        {
            CurrentProduct = current == null ? null : current.ToPublicDto(),
            Queue = queue.Select(p => p.ToPublicDto()).ToList(),
            Bids = bids.Select(b => b.ToPublicDto()).ToList(),
            TimeLeftMs = timeLeftMs,
            CurrentPrice = current?.HuidigePrijs ?? 0m,
            HighestBidderId = bids.FirstOrDefault()?.KoperId
        });
    }
}
