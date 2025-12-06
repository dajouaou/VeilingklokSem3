using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VM.Services;

public sealed class VMService : IVMService
{
    //dependency injection
    private readonly MyContext _db;

    public VMService(MyContext db)
    {
        _db = db;
    }

    //start een veiling
    public async Task<Result<Core.Entities.Veiling>> StartVeilingAsync(int veilingId)
    {
        //haal veiling op
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        //bestaat hij?
        if (veiling == null) return Result<Core.Entities.Veiling>.Fail("Veiling bestaat niet.");
        //current product is gevuld
        if (veiling.CurrentVeilingProductId != null) return Result<Core.Entities.Veiling>.Fail("Veiling is al gestart.");

        
        //zoek het eerste product in de queue
        var first = await _db.VeilingProducten
            .Where(p => p.VeilingId == veilingId)
            .OrderBy(p => p.Volgorde)
            .FirstOrDefaultAsync();

        
        //geen product=fout
        if (first == null) return Result<Core.Entities.Veiling>.Fail("Geen producten gekoppeld.");

        //activeer het eerste product
        first.Status = VeilingProductStatus.Active;
        first.ActivatedAtUtc = DateTime.UtcNow;
        veiling.CurrentVeilingProductId = first.Id;

        //Opslaan
        await _db.SaveChangesAsync();

        
        //Laad veiling voor het dashboard
        var loaded = await LoadDashboardVeilingAsync(veilingId);
        return loaded == null ? Result<Core.Entities.Veiling>.Fail("Veiling niet gevonden.") : Result<Core.Entities.Veiling>.Ok(loaded);
    }

    
    //activeer het volgende product
    public async Task<Result<VeilingProduct>> ActivateNextProductAsync(int veilingId)
    {
        //Haal veiling op
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        
        //Bestaat hij?
        if (veiling == null) return Result<VeilingProduct>.Fail("Veiling niet gevonden.");

        
        //Sluit het huidige product
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
        
        
        //geen current?pak het eerste product

        var currentVolgorde = current?.Volgorde ?? -1;

        var next = await _db.VeilingProducten
            .Where(p => p.VeilingId == veilingId && p.Volgorde > currentVolgorde)
            .OrderBy(p => p.Volgorde)
            .FirstOrDefaultAsync();

        
        //Geen volgende?
        if (next == null)
        {
            veiling.CurrentVeilingProductId = null;
            await _db.SaveChangesAsync();
            return Result<VeilingProduct>.Fail("Geen volgende producten.");
        }

        
        
        //Activeer volgende product
        next.Status = VeilingProductStatus.Active;
        next.ActivatedAtUtc = DateTime.UtcNow;

        veiling.CurrentVeilingProductId = next.Id;

        
        //Opslaan en teruggeven
        await _db.SaveChangesAsync();
        return Result<VeilingProduct>.Ok(next);
    }
    
    

    //Huidig product sluiten
    public async Task<Result<bool>> CloseCurrentProductAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null) return Result<bool>.Fail("Veiling niet gevonden.");

        if (veiling.CurrentVeilingProductId == null)
            return Result<bool>.Fail("Geen actief product.");

        var current = await _db.VeilingProducten.FirstOrDefaultAsync(p => p.Id == veiling.CurrentVeilingProductId);
        if (current == null) return Result<bool>.Fail("Geen actief product.");

        
        //Markeer als Sold
        current.Status = VeilingProductStatus.Sold;
        current.ClosedAtUtc = DateTime.UtcNow;

        
        //reset current op veiling
        veiling.CurrentVeilingProductId = null;

        //Opslaan
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    //Dashboard state ophalen
    public async Task<Result<Core.Entities.Veiling>> GetDashboardStateAsync(int veilingId)
    {
        var veiling = await LoadDashboardVeilingAsync(veilingId);
        if (veiling == null) return Result<Core.Entities.Veiling>.Fail("Veiling niet gevonden.");
        return Result<Core.Entities.Veiling>.Ok(veiling);
    }

    ////alles laden wat het dashboard toont
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