// src/Features/Veiling/Services/VeilingService.cs
using System;
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

public sealed class VeilingService : IVeilingService
{
    private readonly MyContext _db;

    public VeilingService(MyContext db)
    {
        _db = db;
    }

    public async Task<Result<PublicVeilingDto>> LoadPublicAsync(int veilingId)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Product)
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Aanvoerder)
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Bids)
                    .ThenInclude(b => b.Koper)
                    .ThenInclude(k => k.Gebruiker)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<PublicVeilingDto>.Fail("Veiling niet gevonden.");

        var current = veiling.VeilingProducten
            .FirstOrDefault(p => p.Status == VeilingProductStatus.Active);

        var queue = veiling.VeilingProducten
            .Where(p => p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .ToList();

        var bids = current?.Bids
                        .OrderByDescending(b => b.PlacedAtUtc)
                        .ToList()
                    ?? new List<Bid>();

        var dto = new PublicVeilingDto
        {
            CurrentProduct = current?.ToPublicDto(),
            Queue = queue.Select(p => p.ToPublicDto()).ToList(),
            Bids = bids.Select(b => b.ToPublicDto()).ToList(),
            TimeLeftMs = 0,
            CurrentPrice = current?.HuidigePrijs ?? 0m,
            HighestBidderId = bids.FirstOrDefault()?.KoperId
        };

        return Result<PublicVeilingDto>.Ok(dto);
    }

    public async Task<Result<BidResultDto>> PlaceBidAsync(int veilingId, int koperId)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Bids)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<BidResultDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Running)
            return Result<BidResultDto>.Fail("Veiling is niet actief.");

        var current = veiling.VeilingProducten
            .FirstOrDefault(p => p.Status == VeilingProductStatus.Active);

        if (current == null)
            return Result<BidResultDto>.Fail("Er is momenteel geen actief product.");

        var koper = await _db.Kopers
            .Include(k => k.Gebruiker)
            .FirstOrDefaultAsync(k => k.Id == koperId);

        if (koper == null)
            return Result<BidResultDto>.Fail("Koper niet gevonden.");

        var bid = new Bid
        {
            VeilingId = veiling.Id,
            VeilingProductId = current.Id,
            PlacedByGebruikerId = koper.GebruikerId,
            KoperId = koperId,
            Amount = current.HuidigePrijs,
            PlacedAtUtc = DateTime.UtcNow
        };

        current.Bids.Add(bid);
        await _db.SaveChangesAsync();

        var resultDto = new BidResultDto
        {
            Accepted = true,
            Message = "Bod geplaatst.",
            NewPrice = bid.Amount,
            HighestBidderId = koperId
        };

        return Result<BidResultDto>.Ok(resultDto);
    }
}
