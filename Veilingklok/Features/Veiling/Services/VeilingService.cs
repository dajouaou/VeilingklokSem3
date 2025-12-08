// Veilingklok/Features/Veiling/Services/VeilingService.cs
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services;

public sealed class VeilingService : IVeilingService
{
    private readonly MyContext _db;

    public VeilingService(MyContext db)
    {
        _db = db;
    }

    public async Task<Result<VeilingProduct>> GetCurrentProductAsync(int veilingId)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.CurrentVeilingProduct)
                .ThenInclude(p => p.Product)
            .Include(v => v.CurrentVeilingProduct)
                .ThenInclude(p => p.Aanvoerder)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<VeilingProduct>.Fail("Veiling niet gevonden.");

        if (veiling.CurrentVeilingProduct == null)
            return Result<VeilingProduct>.Fail("Geen actief product.");

        return Result<VeilingProduct>.Ok(veiling.CurrentVeilingProduct);
    }

    public async Task<Result<List<VeilingProduct>>> GetQueueAsync(int veilingId)
    {
        var producten = await _db.VeilingProducten
            .Include(p => p.Product)
            .Include(p => p.Aanvoerder)
            .Where(p => p.VeilingId == veilingId && p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .ToListAsync();

        return Result<List<VeilingProduct>>.Ok(producten);
    }

    public async Task<Result<Bid>> PlaceBidAsync(int veilingId, int koperId, decimal amount)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.CurrentVeilingProduct)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<Bid>.Fail("Veiling niet gevonden.");

        var current = veiling.CurrentVeilingProduct;
        if (current == null || current.Status != VeilingProductStatus.Active)
            return Result<Bid>.Fail("Bieden is niet mogelijk.");

        var koper = await _db.Kopers
            .Include(k => k.Gebruiker)
            .FirstOrDefaultAsync(k => k.Id == koperId);

        if (koper == null)
            return Result<Bid>.Fail("Koper niet gevonden.");

        var bid = new Bid
        {
            VeilingId = veilingId,
            VeilingProductId = current.Id,
            KoperId = koper.Id,
            PlacedByGebruikerId = koper.GebruikerId,
            Amount = amount,
            Source = BidSource.Buyer
        };

        current.Bids.Add(bid);
        current.HuidigePrijs = amount;

        await _db.SaveChangesAsync();

        return Result<Bid>.Ok(bid);
    }
}
