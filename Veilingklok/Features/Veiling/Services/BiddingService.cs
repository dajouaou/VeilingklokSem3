using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Services;

public sealed class BiddingService : IBiddingService
{
    private const int DefaultDurationSeconds = 20;
    private static readonly TimeSpan BidCooldown = TimeSpan.FromMilliseconds(400);

    private readonly MyContext _db;
    private readonly IVeilingBroadcastService _broadcast;

    public BiddingService(MyContext db, IVeilingBroadcastService broadcast)
    {
        _db = db;
        _broadcast = broadcast;
    }

    public async Task<Result<BidResultDto>> PlaceBidAsync(int veilingId, int gebruikerId, decimal? price)
    {
        var now = DateTime.UtcNow;

        var koper = await _db.Kopers
            .Include(k => k.Gebruiker)
            .FirstOrDefaultAsync(k => k.GebruikerId == gebruikerId);

        if (koper == null)
            return Result<BidResultDto>.Fail("Koper niet gevonden.");

        var veiling = await _db.Veilingen
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Aanmelding)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<BidResultDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Running)
            return Result<BidResultDto>.Fail("Veiling is niet actief.");

        if (veiling.CurrentVeilingProductId == null)
            return Result<BidResultDto>.Fail("Er is momenteel geen actief product.");

        var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId.Value);

        if (current == null || current.Status != VeilingProductStatus.Active)
            return Result<BidResultDto>.Fail("Er is momenteel geen actief product.");

        if (current.ClosedAtUtc != null)
            return Result<BidResultDto>.Fail("Dit product is al gesloten.");

        var lastBidTime = await _db.Bids
            .AsNoTracking()
            .Where(b => b.VeilingId == veilingId && b.VeilingProductId == current.Id && b.KoperId == koper.Id)
            .OrderByDescending(b => b.PlacedAtUtc)
            .Select(b => (DateTime?)b.PlacedAtUtc)
            .FirstOrDefaultAsync();

        if (lastBidTime != null && (now - lastBidTime.Value) < BidCooldown)
            return Result<BidResultDto>.Fail("Te snel achter elkaar geboden.");

        var acceptedAmount = current.HuidigePrijs;

        if (price.HasValue && price.Value != acceptedAmount)
            return Result<BidResultDto>.Fail("Prijs komt niet overeen met huidige prijs.");

        await using var tx = await _db.Database.BeginTransactionAsync();

        var bid = new Bid
        {
            VeilingId = veiling.Id,
            VeilingProductId = current.Id,
            KoperId = koper.Id,
            Amount = acceptedAmount,
            PlacedAtUtc = now
        };

        _db.Bids.Add(bid);

        current.KoperId = koper.Id;
        current.Status = VeilingProductStatus.Sold;
        current.ClosedAtUtc = now;

        _db.AuditEntries.Add(new AuditEntry
        {
            VeilingId = veiling.Id,
            ActorGebruikerId = koper.GebruikerId,
            Action = $"Bod geplaatst en product verkocht (veilingProduct {current.Id}, koper {koper.Id}, prijs {bid.Amount})",
            CreatedAtUtc = now
        });

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

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var bodDto = new BodDto
        {
            Id = bid.Id,
            Prijs = bid.Amount,
            KoperNaam = koper.Gebruiker == null ? null : $"{koper.Gebruiker.Voornaam} {koper.Gebruiker.Achternaam}".Trim(),
            Tijdstip = bid.PlacedAtUtc
        };

        var biedingen = await _db.Bids
            .AsNoTracking()
            .Include(b => b.Koper)
                .ThenInclude(k => k!.Gebruiker)
            .Where(b => b.VeilingId == veilingId && b.VeilingProductId == current.Id)
            .OrderByDescending(b => b.PlacedAtUtc)
            .Select(b => new BodDto
            {
                Id = b.Id,
                Prijs = b.Amount,
                KoperNaam = b.Koper!.Gebruiker == null ? null : $"{b.Koper.Gebruiker.Voornaam} {b.Koper.Gebruiker.Achternaam}".Trim(),
                Tijdstip = b.PlacedAtUtc
            })
            .ToListAsync();

        await _broadcast.StuurBod(veilingId, bodDto);
        await _broadcast.StuurBiedingen(veilingId, biedingen);

        var wachtrij = veiling.VeilingProducten
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
            .ToList();

        await _broadcast.StuurWachtrij(veilingId, wachtrij);

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

            var totalSeconds = next.DurationSeconds > 0 ? next.DurationSeconds : DefaultDurationSeconds;

            await _broadcast.StuurTick(veilingId, new TickDto
            {
                VeilingId = veilingId,
                VeilingProductId = next.Id,
                CurrentPrice = next.HuidigePrijs,
                TimeLeftMs = totalSeconds * 1000
            });
        }
        else
        {
            await _broadcast.StuurTick(veilingId, new TickDto
            {
                VeilingId = veilingId,
                VeilingProductId = null,
                CurrentPrice = 0m,
                TimeLeftMs = 0
            });
        }

        return Result<BidResultDto>.Ok(new BidResultDto
        {
            Accepted = true,
            Message = "Bod geplaatst en product verkocht.",
            NewPrice = bid.Amount,
            HighestBidderId = koper.Id
        });
    }
}
