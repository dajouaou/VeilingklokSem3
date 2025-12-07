using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VM.Services;

public sealed class VMService : IVMService
{
    private readonly MyContext _db;

    public VMService(MyContext db)
    {
        _db = db;
    }

    public async Task<Result<VMVeilingDashboardDto>> GetDashboardAsync(int veilingId)
    {
        return await BuildDashboardResultAsync(veilingId);
    }

    public async Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null)
            return Result<VMVeilingDashboardDto>.Fail("Veiling bestaat niet.");

        if (veiling.CurrentVeilingProductId != null)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al gestart.");

        var first = await _db.VeilingProducten
            .Where(p => p.VeilingId == veilingId)
            .OrderBy(p => p.Volgorde)
            .FirstOrDefaultAsync();

        if (first == null)
            return Result<VMVeilingDashboardDto>.Fail("Geen producten gekoppeld.");

        first.Status = VeilingProductStatus.Active;
        first.ActivatedAtUtc = DateTime.UtcNow;
        veiling.CurrentVeilingProductId = first.Id;

        await _db.SaveChangesAsync();

        return await BuildDashboardResultAsync(veilingId);
    }

    public async Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null)
            return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

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
            return Result<VMVeilingDashboardDto>.Fail("Geen volgende producten.");
        }

        next.Status = VeilingProductStatus.Active;
        next.ActivatedAtUtc = DateTime.UtcNow;
        veiling.CurrentVeilingProductId = next.Id;

        await _db.SaveChangesAsync();

        return await BuildDashboardResultAsync(veilingId);
    }

    public async Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId)
    {
        var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
        if (veiling == null)
            return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.CurrentVeilingProductId == null)
            return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

        var current = await _db.VeilingProducten.FirstOrDefaultAsync(p => p.Id == veiling.CurrentVeilingProductId);
        if (current == null)
            return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

        current.Status = VeilingProductStatus.Sold;
        current.ClosedAtUtc = DateTime.UtcNow;
        veiling.CurrentVeilingProductId = null;

        await _db.SaveChangesAsync();

        return await BuildDashboardResultAsync(veilingId);
    }

    private async Task<Result<VMVeilingDashboardDto>> BuildDashboardResultAsync(int veilingId)
    {
        var veiling = await LoadDashboardVeilingAsync(veilingId);
        if (veiling == null)
            return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        var dto = VMVeilingDashboardDto.FromEntity(veiling);
        return Result<VMVeilingDashboardDto>.Ok(dto);
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
