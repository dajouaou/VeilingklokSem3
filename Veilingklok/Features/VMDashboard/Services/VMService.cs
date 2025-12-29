// Veilingklok/Features/VM/Services/VMService.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;

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

    public async Task<Result<VMActiveVeilingDto?>> GetActieveVeilingAsync()
    {
        var actieve = await _db.Veilingen
            .AsNoTracking()
            .Include(v => v.CurrentVeilingProduct)
            .ThenInclude(vp => vp!.Product)
            .Include(v => v.CurrentVeilingProduct)
            .ThenInclude(vp => vp!.Aanvoerder)
            .Where(v => v.Status == VeilingStatus.Running || v.Status == VeilingStatus.Paused)
            .OrderByDescending(v => v.StartTijdUtc ?? DateTime.MinValue)
            .ThenByDescending(v => v.Id)
            .FirstOrDefaultAsync();

        return Result<VMActiveVeilingDto?>.Ok(actieve == null ? null : VMActiveVeilingDto.FromEntity(actieve));
    }

    public async Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status == VeilingStatus.Running || veiling.Status == VeilingStatus.Paused)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is al gestart.", ErrorCode.Conflict);

            if (veiling.Status == VeilingStatus.Finished)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is al beëindigd.", ErrorCode.Conflict);

            if (!MagNuStartenUtc(veiling))
            {
                var geplandeStartLocal = GetGeplandeStartLocal(veiling);
                return Result<VMVeilingDashboardDto>.Fail($"Veiling kan pas starten op {geplandeStartLocal:yyyy-MM-dd HH:mm}", ErrorCode.Conflict);
            }

            var first = veiling.VeilingProducten
                .Where(p => p.Status == VeilingProductStatus.Queued)
                .OrderBy(p => p.Volgorde)
                .ThenBy(p => p.Id)
                .FirstOrDefault();

            if (first == null)
                return Result<VMVeilingDashboardDto>.Fail("Geen producten gekoppeld.", ErrorCode.Conflict);

            first.Status = VeilingProductStatus.Active;
            first.ActivatedAtUtc = DateTime.UtcNow;

            veiling.CurrentVeilingProductId = first.Id;
            veiling.Status = VeilingStatus.Running;
            veiling.StartTijdUtc ??= DateTime.UtcNow;

            AddAuditEntry(veiling, "Veiling gestart", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> PauseVeilingAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status != VeilingStatus.Running)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is niet running.", ErrorCode.Conflict);

            veiling.Status = VeilingStatus.Paused;
            AddAuditEntry(veiling, "Veiling gepauzeerd", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ResumeVeilingAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status != VeilingStatus.Paused)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is niet gepauzeerd.", ErrorCode.Conflict);

            veiling.Status = VeilingStatus.Running;
            AddAuditEntry(veiling, "Veiling hervat", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> StopVeilingAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status == VeilingStatus.Finished)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is al beëindigd.", ErrorCode.Conflict);

            CloseCurrentIfActive(veiling);

            veiling.CurrentVeilingProductId = null;
            veiling.Status = VeilingStatus.Finished;
            veiling.EindTijdUtc ??= DateTime.UtcNow;

            AddAuditEntry(veiling, "Veiling gestopt", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status == VeilingStatus.Paused)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.", ErrorCode.Conflict);

            if (veiling.Status == VeilingStatus.Finished)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.", ErrorCode.Conflict);

            var current = CloseCurrentIfActive(veiling);
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

                AddAuditEntry(veiling, "Geen volgende producten, veiling beëindigd", actorGebruikerId);

                await _db.SaveChangesAsync();
                return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
            }

            next.Status = VeilingProductStatus.Active;
            next.ActivatedAtUtc = DateTime.UtcNow;

            veiling.CurrentVeilingProductId = next.Id;
            veiling.Status = VeilingStatus.Running;

            AddAuditEntry(veiling, "Volgend product geactiveerd", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status == VeilingStatus.Paused)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.", ErrorCode.Conflict);

            if (veiling.Status == VeilingStatus.Finished)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.", ErrorCode.Conflict);

            if (veiling.CurrentVeilingProductId == null)
                return Result<VMVeilingDashboardDto>.Fail("Geen actief product.", ErrorCode.Conflict);

            var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);
            if (current == null || current.Status != VeilingProductStatus.Active)
                return Result<VMVeilingDashboardDto>.Fail("Geen actief product.", ErrorCode.Conflict);

            current.Status = VeilingProductStatus.Sold;
            current.ClosedAtUtc = DateTime.UtcNow;

            veiling.CurrentVeilingProductId = null;

            if (!HasOpenProducts(veiling))
            {
                veiling.Status = VeilingStatus.Finished;
                veiling.EindTijdUtc ??= DateTime.UtcNow;
            }

            AddAuditEntry(veiling, "Huidig product gesloten", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ResetVeilingAsync(int veilingId, int actorGebruikerId)
    {
        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

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

            AddAuditEntry(veiling, "Veiling gereset", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ReorderQueueAsync(int veilingId, VMReorderQueueRequest request, int actorGebruikerId)
    {
        if (request.OrderedVeilingProductIds == null || request.OrderedVeilingProductIds.Length == 0)
            return Result<VMVeilingDashboardDto>.Fail("Geen ids meegegeven.", ErrorCode.Validation);

        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status == VeilingStatus.Running || veiling.Status == VeilingStatus.Paused)
                return Result<VMVeilingDashboardDto>.Fail("Queue reorder kan niet tijdens running/paused.", ErrorCode.Conflict);

            var queued = veiling.VeilingProducten
                .Where(p => p.Status == VeilingProductStatus.Queued)
                .ToList();

            var queuedIds = queued.Select(x => x.Id).ToHashSet();
            var incomingIds = request.OrderedVeilingProductIds.ToHashSet();

            if (!incomingIds.SetEquals(queuedIds))
                return Result<VMVeilingDashboardDto>.Fail("Reorder ids matchen niet met de huidige queue.", ErrorCode.Conflict);

            var volgorde = 1;
            foreach (var id in request.OrderedVeilingProductIds)
                queued.First(x => x.Id == id).Volgorde = volgorde++;

            AddAuditEntry(veiling, "Queue volgorde aangepast", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> SkipProductAsync(int veilingId, int veilingProductId, int actorGebruikerId)
    {
        var shouldActivateNext = false;

        var txResult = await ExecuteInTransactionAsync(async () =>
        {
            var veiling = await LoadVeilingForMutationAsync(veilingId);
            if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);

            if (veiling.Status == VeilingStatus.Paused)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.", ErrorCode.Conflict);

            if (veiling.Status == VeilingStatus.Finished)
                return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.", ErrorCode.Conflict);

            var product = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veilingProductId);
            if (product == null) return Result<VMVeilingDashboardDto>.Fail("Product niet gevonden.", ErrorCode.NotFound);

            var maxVolgorde = veiling.VeilingProducten.Max(p => p.Volgorde);

            if (veiling.CurrentVeilingProductId == veilingProductId && product.Status == VeilingProductStatus.Active)
            {
                product.Status = VeilingProductStatus.Skipped;
                product.ClosedAtUtc = DateTime.UtcNow;
                veiling.CurrentVeilingProductId = null;

                AddAuditEntry(veiling, "Huidig product geskipt (gesloten)", actorGebruikerId);

                await _db.SaveChangesAsync();
                shouldActivateNext = true;
                return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
            }

            if (product.Status != VeilingProductStatus.Queued)
                return Result<VMVeilingDashboardDto>.Fail("Alleen queued producten kun je skippen.", ErrorCode.Conflict);

            product.Volgorde = maxVolgorde + 1;

            AddAuditEntry(veiling, "Product geskipt (naar achter gezet)", actorGebruikerId);

            await _db.SaveChangesAsync();
            return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
        });

        if (!txResult.Success) return txResult;

        if (shouldActivateNext)
            return await ActivateNextProductAsync(veilingId, actorGebruikerId);

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<List<string>>> GetVeildagenAsync()
        => Result<List<string>>.Fail("Niet meer via VMService; gebruik planning service.", ErrorCode.Conflict);

    public async Task<Result<List<VeilingPlanningAanmeldingDto>>> GetAanmeldingenAsync(string leverdatum)
        => Result<List<VeilingPlanningAanmeldingDto>>.Fail("Niet meer via VMService; gebruik planning service.", ErrorCode.Conflict);

    public async Task<Result<int>> PlanVeilingAsync(PlanVeilingRequestDto dto, int actorGebruikerId)
        => Result<int>.Fail("Niet meer via VMService; gebruik planning service.", ErrorCode.Conflict);

    public async Task<Result<List<GeplandeVeilingListItemDto>>> GetGeplandeAsync()
        => Result<List<GeplandeVeilingListItemDto>>.Fail("Niet meer via VMService; gebruik planning service.", ErrorCode.Conflict);

    public async Task<Result<GeplandeVeilingListItemDto?>> GetVolgendeGeplandeAsync()
        => Result<GeplandeVeilingListItemDto?>.Fail("Niet meer via VMService; gebruik planning service.", ErrorCode.Conflict);

    private async Task<Result<VMVeilingDashboardDto>> BuildDashboardResultAsync(int veilingId)
    {
        var veiling = await LoadDashboardVeilingAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.", ErrorCode.NotFound);
        return Result<VMVeilingDashboardDto>.Ok(VMVeilingDashboardDto.FromEntity(veiling));
    }

    private Task<VeilingEntity?> LoadVeilingForMutationAsync(int veilingId)
    {
        return _db.Veilingen
            .Include(v => v.VM)
            .Include(v => v.VeilingProducten)
            .Include(v => v.AuditEntries)
            .FirstOrDefaultAsync(v => v.Id == veilingId);
    }

    private Task<VeilingEntity?> LoadDashboardVeilingAsync(int veilingId)
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

    private static bool HasOpenProducts(VeilingEntity veiling)
    {
        return veiling.VeilingProducten.Any(p =>
            p.Status == VeilingProductStatus.Queued ||
            p.Status == VeilingProductStatus.Active);
    }

    private static VeilingProduct? CloseCurrentIfActive(VeilingEntity veiling)
    {
        if (veiling.CurrentVeilingProductId == null) return null;

        var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);
        if (current == null) return null;

        if (current.Status == VeilingProductStatus.Active)
        {
            current.Status = VeilingProductStatus.Skipped;
            current.ClosedAtUtc = DateTime.UtcNow;
        }

        return current;
    }

    private static DateTime GetGeplandeStartLocal(VeilingEntity veiling)
    {
        var local = veiling.Datum.Date + veiling.StartTijd;
        return DateTime.SpecifyKind(local, DateTimeKind.Local);
    }

    private static bool MagNuStartenUtc(VeilingEntity veiling)
    {
        var geplandeLocal = GetGeplandeStartLocal(veiling);
        var geplandeUtc = geplandeLocal.ToUniversalTime();
        return DateTime.UtcNow >= geplandeUtc;
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

    private static void AddAuditEntry(VeilingEntity veiling, string action, int actorGebruikerId)
    {
        veiling.AuditEntries ??= new List<AuditEntry>();

        veiling.AuditEntries.Add(new AuditEntry
        {
            VeilingId = veiling.Id,
            Action = action,
            CreatedAtUtc = DateTime.UtcNow,
            ActorGebruikerId = actorGebruikerId
        });
    }

    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            var result = await action();
            await tx.CommitAsync();
            return result;
        });
    }
}
