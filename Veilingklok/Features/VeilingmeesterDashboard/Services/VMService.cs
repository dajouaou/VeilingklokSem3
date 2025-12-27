// Veilingklok/Features/VM/Services/VMService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.VM.Services;

public sealed class VMService : IVMService
{
    private readonly MyContext _db;
    private readonly IVeilingBroadcastService _broadcast;

    public VMService(MyContext db, IVeilingBroadcastService broadcast)
    {
        _db = db;
        _broadcast = broadcast;
    }

    public Task<Result<VMVeilingDashboardDto>> GetDashboardAsync(int veilingId)
        => BuildDashboardResultAsync(veilingId);

    public async Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Running || veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al gestart.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al beëindigd.");

        if (!MagNuStarten(veiling))
        {
            var geplandeStart = GetGeplandeStart(veiling);
            return Result<VMVeilingDashboardDto>.Fail($"Veiling kan pas starten op {geplandeStart:yyyy-MM-dd HH:mm}");
        }

        var first = veiling.VeilingProducten
            .Where(p => p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .ThenBy(p => p.Id)
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

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> PauseVeilingAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Running)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is niet running.");

        veiling.Status = VeilingStatus.Paused;

        AddAuditEntry(veiling, "Veiling gepauzeerd");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ResumeVeilingAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is niet gepauzeerd.");

        veiling.Status = VeilingStatus.Running;

        AddAuditEntry(veiling, "Veiling hervat");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> StopVeilingAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al beëindigd.");

        if (veiling.CurrentVeilingProductId != null)
        {
            var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);
            if (current != null && current.Status == VeilingProductStatus.Active)
            {
                current.Status = VeilingProductStatus.Sold;
                current.ClosedAtUtc = DateTime.UtcNow;
            }
        }

        veiling.CurrentVeilingProductId = null;
        veiling.Status = VeilingStatus.Finished;
        veiling.EindTijdUtc ??= DateTime.UtcNow;

        AddAuditEntry(veiling, "Veiling gestopt");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.");

        VeilingProduct? current = null;

        if (veiling.CurrentVeilingProductId != null)
        {
            current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);
            if (current != null && current.Status == VeilingProductStatus.Active)
            {
                current.Status = VeilingProductStatus.Sold;
                current.ClosedAtUtc = DateTime.UtcNow;
            }
        }

        var currentVolgorde = current?.Volgorde ?? -1;

        var next = veiling.VeilingProducten
            .Where(p => p.Status == VeilingProductStatus.Queued && p.Volgorde > currentVolgorde)
            .OrderBy(p => p.Volgorde)
            .ThenBy(p => p.Id)
            .FirstOrDefault();

        if (next == null)
        {
            veiling.CurrentVeilingProductId = null;
            veiling.Status = VeilingStatus.Finished;
            veiling.EindTijdUtc ??= DateTime.UtcNow;

            AddAuditEntry(veiling, "Geen volgende producten, veiling beëindigd");

            await _db.SaveChangesAsync();

            var dashFinal = await BuildDashboardResultAsync(veilingId);
            if (dashFinal.Success)
                await BroadcastDashboardAsync(veilingId, dashFinal.Value!);

            return dashFinal;
        }

        next.Status = VeilingProductStatus.Active;
        next.ActivatedAtUtc = DateTime.UtcNow;

        veiling.CurrentVeilingProductId = next.Id;
        veiling.Status = VeilingStatus.Running;

        AddAuditEntry(veiling, "Volgend product geactiveerd");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.");

        if (veiling.CurrentVeilingProductId == null)
            return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

        var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);
        if (current == null) return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

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

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ResetVeilingAsync(int veilingId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        foreach (var p in veiling.VeilingProducten)
        {
            p.Status = VeilingProductStatus.Queued;
            p.ActivatedAtUtc = null;
            p.ClosedAtUtc = null;
            p.HuidigePrijs = p.StartPrijs;
        }

        veiling.CurrentVeilingProductId = null;
        veiling.StartTijdUtc = null;
        veiling.EindTijdUtc = null;
        veiling.Status = VeilingStatus.Scheduled;

        AddAuditEntry(veiling, "Veiling gereset");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ReorderQueueAsync(int veilingId, VMReorderQueueRequest request)
    {
        if (request.OrderedVeilingProductIds.Length == 0)
            return Result<VMVeilingDashboardDto>.Fail("Geen ids meegegeven.");

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Running || veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Queue reorder kan niet tijdens running/paused.");

        var queued = veiling.VeilingProducten
            .Where(p => p.Status == VeilingProductStatus.Queued)
            .ToList();

        var queuedIds = queued.Select(x => x.Id).ToHashSet();
        var incomingIds = request.OrderedVeilingProductIds.ToHashSet();

        if (!incomingIds.SetEquals(queuedIds))
            return Result<VMVeilingDashboardDto>.Fail("Reorder ids matchen niet met de huidige queue.");

        var volgorde = 1;
        foreach (var id in request.OrderedVeilingProductIds)
        {
            var p = queued.First(x => x.Id == id);
            p.Volgorde = volgorde++;
        }

        AddAuditEntry(veiling, "Queue volgorde aangepast");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> SkipProductAsync(int veilingId, int veilingProductId)
    {
        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.");

        var product = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veilingProductId);
        if (product == null) return Result<VMVeilingDashboardDto>.Fail("Product niet gevonden.");

        var maxVolgorde = veiling.VeilingProducten.Max(p => p.Volgorde);

        if (veiling.CurrentVeilingProductId == veilingProductId && product.Status == VeilingProductStatus.Active)
        {
            product.Status = VeilingProductStatus.Sold;
            product.ClosedAtUtc = DateTime.UtcNow;
            veiling.CurrentVeilingProductId = null;

            AddAuditEntry(veiling, "Huidig product geskipt (gesloten)");

            await _db.SaveChangesAsync();

            var dashboardAfter = await ActivateNextProductAsync(veilingId);
            if (dashboardAfter.Success)
                await BroadcastDashboardAsync(veilingId, dashboardAfter.Value!);

            return dashboardAfter;
        }

        if (product.Status != VeilingProductStatus.Queued)
            return Result<VMVeilingDashboardDto>.Fail("Alleen queued producten kun je skippen.");

        product.Volgorde = maxVolgorde + 1;

        AddAuditEntry(veiling, "Product geskipt (naar achter gezet)");

        await _db.SaveChangesAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success)
            await BroadcastDashboardAsync(veilingId, dashboard.Value!);

        return dashboard;
    }

    private async Task<Result<VMVeilingDashboardDto>> BuildDashboardResultAsync(int veilingId)
    {
        var veiling = await LoadDashboardVeilingAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
    }

    private Task<Core.Entities.Veiling?> LoadVeilingForMutationAsync(int veilingId)
    {
        return _db.Veilingen
            .Include(v => v.VM)
            .Include(v => v.VeilingProducten)
            .FirstOrDefaultAsync(v => v.Id == veilingId);
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

    private static DateTime GetGeplandeStart(Core.Entities.Veiling veiling)
    {
        return veiling.Datum.Date + veiling.StartTijd;
    }

    private static bool MagNuStarten(Core.Entities.Veiling veiling)
    {
        var geplandeStart = GetGeplandeStart(veiling);
        return DateTime.Now >= geplandeStart;
    }

    private async Task BroadcastDashboardAsync(int veilingId, VMVeilingDashboardDto dto)
    {
        if (dto.Current != null)
            await _broadcast.StuurHuidigProduct(veilingId, MapToHuidigProductDto(dto.Current));

        await _broadcast.StuurWachtrij(veilingId, MapToWachtrijDto(dto.Queue));
    }

    private static HuidigProductDto MapToHuidigProductDto(VMCurrentProductDto dto)
    {
        return new HuidigProductDto
        {
            VeilingProductId = dto.Id,
            Soort = dto.ProductNaam,
            FotoUrl = dto.FotoUrl,
            StartPrijs = dto.StartPrijs,
            HuidigePrijs = dto.HuidigePrijs,
            Hoeveelheid = dto.Hoeveelheid,
            IsActief = dto.Status == VeilingProductStatus.Active,
            IsVerkocht = dto.Status == VeilingProductStatus.Sold
        };
    }

    private static List<WachtrijItemDto> MapToWachtrijDto(List<VMQueueItemDto> queue)
    {
        return queue.Select(x => new WachtrijItemDto
        {
            VeilingProductId = x.Id,
            Volgorde = x.Volgorde,
            Soort = x.ProductNaam,
            FotoUrl = x.FotoUrl,
            StartPrijs = x.StartPrijs,
            Hoeveelheid = x.Hoeveelheid
        }).ToList();
    }

    private static void AddAuditEntry(Core.Entities.Veiling veiling, string action)
    {
        veiling.AuditEntries ??= new List<AuditEntry>();

        veiling.AuditEntries.Add(new AuditEntry
        {
            VeilingId = veiling.Id,
            Action = action,
            CreatedAtUtc = DateTime.UtcNow,
            ActorGebruikerId = veiling.VM?.GebruikerId ?? 1
        });
    }
}
