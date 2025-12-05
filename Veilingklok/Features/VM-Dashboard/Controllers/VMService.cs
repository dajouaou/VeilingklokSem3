using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VM.Services;

public sealed class VMService : IVMService
{
    private readonly MyContext _db;

    public VMService(MyContext db)
    {
        _db = db;
    }

    public async Task<Result<Core.Entities.Veiling>> StartVeilingAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null) return Result<Core.Entities.Veiling>.Fail("Veiling bestaat niet.");
        if (veiling.CurrentVeilingProductId != null) return Result<Core.Entities.Veiling>.Fail("Veiling is al gestart.");

        var first = await _db.VeilingProducten
            .Where(p => p.VeilingId == veilingId)
            .OrderBy(p => p.Volgorde)
            .FirstOrDefaultAsync();

        if (first == null) return Result<Core.Entities.Veiling>.Fail("Geen producten gekoppeld.");

        first.Status = VeilingProductStatus.Active;
        first.ActivatedAtUtc = DateTime.UtcNow;
        veiling.CurrentVeilingProductId = first.Id;

        await _db.SaveChangesAsync();

        var loaded = await LoadDashboardVeilingAsync(veilingId);
        return loaded == null ? Result<Core.Entities.Veiling>.Fail("Veiling niet gevonden.") : Result<Core.Entities.Veiling>.Ok(loaded);
    }

    public async Task<Result<VeilingProduct>> ActivateNextProductAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null) return Result<VeilingProduct>.Fail("Veiling niet gevonden.");

        VeilingProduct? current = null;

        if (veiling.CurrentVeilingProductId != null)
        {
            current = await _db.VeilingProducten.FirstOrDefaultAsync(p => p.Id == veiling.CurrentVeilingProductId);
            if (current != null && current.Status == VeilingProductStatus.Active)
            {
                current.Status = VeilingProductStatus.Sold;
                current.ClosedAtUtc = DateTime.UtcNow;
            }
        }

        var currentVolgorde = current?.Volgorde ?? -1;

        var next = await _db.VeilingProducten
            .Where(p => p.VeilingId == veilingId && p.Volgorde > currentVolgorde)
            .OrderBy(p => p.Volgorde)
            .FirstOrDefaultAsync();

        if (next == null)
        {
            veiling.CurrentVeilingProductId = null;
            await _db.SaveChangesAsync();
            return Result<VeilingProduct>.Fail("Geen volgende producten.");
        }

        next.Status = VeilingProductStatus.Active;
        next.ActivatedAtUtc = DateTime.UtcNow;

        veiling.CurrentVeilingProductId = next.Id;

        await _db.SaveChangesAsync();
        return Result<VeilingProduct>.Ok(next);
    }

    public async Task<Result<bool>> CloseCurrentProductAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null) return Result<bool>.Fail("Veiling niet gevonden.");

        if (veiling.CurrentVeilingProductId == null)
            return Result<bool>.Fail("Geen actief product.");

        var current = await _db.VeilingProducten.FirstOrDefaultAsync(p => p.Id == veiling.CurrentVeilingProductId);
        if (current == null) return Result<bool>.Fail("Geen actief product.");

        current.Status = VeilingProductStatus.Sold;
        current.ClosedAtUtc = DateTime.UtcNow;

        veiling.CurrentVeilingProductId = null;

        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    public async Task<Result<Core.Entities.Veiling>> GetDashboardStateAsync(int veilingId)
    {
        var veiling = await LoadDashboardVeilingAsync(veilingId);
        if (veiling == null) return Result<Core.Entities.Veiling>.Fail("Veiling niet gevonden.");
        return Result<Core.Entities.Veiling>.Ok(veiling);
    }

    private Task<Core.Entities.Veiling?> LoadDashboardVeilingAsync(int veilingId)
    {
        return _db.Veilingen
            .Include(v => v.VM)
                .ThenInclude(vm => vm!.Gebruiker)
            .Include(v => v.CurrentVeilingProduct)
                .ThenInclude(vp => vp!.Product)
            .Include(v => v.CurrentVeilingProduct)
                .ThenInclude(vp => vp!.Aanvoerder)
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Product)
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Aanvoerder)
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Bids)
                    .ThenInclude(b => b.Koper)
                        .ThenInclude(k => k!.Gebruiker)
            .Include(v => v.AuditEntries)
                .ThenInclude(a => a.ActorGebruiker)
            .FirstOrDefaultAsync(v => v.Id == veilingId);
    }
}
