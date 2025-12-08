// Veilingklok/Features/VM/Services/VMService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    var veiling = await _db.Veilingen
        .Include(v => v.VeilingProducten)
        .FirstOrDefaultAsync(v => v.Id == veilingId);

    if (veiling == null)
        return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

    if (veiling.CurrentVeilingProductId != null)
        return Result<VMVeilingDashboardDto>.Fail("Veiling is al gestart.");

    var first = veiling.VeilingProducten
        .Where(p => p.Status == VeilingProductStatus.Queued)
        .OrderBy(p => p.Volgorde)
        .FirstOrDefault();

    if (first == null)
        return Result<VMVeilingDashboardDto>.Fail("Geen producten gekoppeld.");

    first.Status = VeilingProductStatus.Active;
    first.ActivatedAtUtc = DateTime.UtcNow;
    veiling.CurrentVeilingProductId = first.Id;

    veiling.Status = VeilingStatus.Running;
    veiling.StartTijdUtc ??= DateTime.UtcNow;

    AddAuditEntry(veiling, "Veiling gestart");

    await _db.SaveChangesAsync();

    return await BuildDashboardResultAsync(veilingId);
}

public async Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId)
{
    var veiling = await _db.Veilingen
        .Include(v => v.VeilingProducten)
        .FirstOrDefaultAsync(v => v.Id == veilingId);

    if (veiling == null)
        return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

    VeilingProduct? current = null;

    if (veiling.CurrentVeilingProductId != null)
    {
        current = veiling.VeilingProducten
            .FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);

        if (current != null && current.Status == VeilingProductStatus.Active)
        {
            current.Status = VeilingProductStatus.Sold;
            current.ClosedAtUtc = DateTime.UtcNow;
        }
    }

    var currentVolgorde = current?.Volgorde ?? -1;

    var next = veiling.VeilingProducten
        .Where(p =>
            p.Volgorde > currentVolgorde &&
            p.Status == VeilingProductStatus.Queued)
        .OrderBy(p => p.Volgorde)
        .FirstOrDefault();

    if (next == null)
    {
        veiling.CurrentVeilingProductId = null;
        veiling.Status = VeilingStatus.Finished;
        veiling.EindTijdUtc ??= DateTime.UtcNow;

        AddAuditEntry(veiling, "Geen volgende producten, veiling beëindigd");

        await _db.SaveChangesAsync();

        return await BuildDashboardResultAsync(veilingId);
    }

    next.Status = VeilingProductStatus.Active;
    next.ActivatedAtUtc = DateTime.UtcNow;
    veiling.CurrentVeilingProductId = next.Id;
    veiling.Status = VeilingStatus.Running;

    AddAuditEntry(veiling, "Volgend product geactiveerd");

    await _db.SaveChangesAsync();

    return await BuildDashboardResultAsync(veilingId);
}

public async Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId)
{
    var veiling = await _db.Veilingen
        .Include(v => v.VeilingProducten)
        .FirstOrDefaultAsync(v => v.Id == veilingId);

    if (veiling == null)
        return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

    if (veiling.CurrentVeilingProductId == null)
        return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

    var current = veiling.VeilingProducten
        .FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);

    if (current == null)
        return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

    current.Status = VeilingProductStatus.Sold;
    current.ClosedAtUtc = DateTime.UtcNow;
    veiling.CurrentVeilingProductId = null;

    if (!HasOpenProducts(veiling))
    {
        veiling.Status = VeilingStatus.Finished;
        veiling.EindTijdUtc ??= DateTime.UtcNow;
    }

    AddAuditEntry(veiling, "Huidig product gesloten");

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
        .AsNoTracking()
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

private static bool HasOpenProducts(Core.Entities.Veiling veiling)
{
    return veiling.VeilingProducten.Any(p =>
        p.Status == VeilingProductStatus.Queued ||
        p.Status == VeilingProductStatus.Active);
}

private static void AddAuditEntry(Core.Entities.Veiling veiling, string action)
{
    veiling.AuditEntries ??= new List<AuditEntry>();

    veiling.AuditEntries.Add(new AuditEntry
    {
        VeilingId = veiling.Id,
        Action = action,
        CreatedAtUtc = DateTime.UtcNow
    });
}
}