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

        return actieve == null
            ? Result<VMActiveVeilingDto?>.Ok(null)
            : Result<VMActiveVeilingDto?>.Ok(VMActiveVeilingDto.FromEntity(actieve));
    }

    public async Task<Result<VMVeilingDashboardDto>> StartVeilingAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Running || veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al gestart.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al beëindigd.");

        if (!MagNuStartenUtc(veiling))
        {
            var geplandeStartLocal = GetGeplandeStartLocal(veiling);
            return Result<VMVeilingDashboardDto>.Fail($"Veiling kan pas starten op {geplandeStartLocal:yyyy-MM-dd HH:mm}");
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

        AddAuditEntry(veiling, "Veiling gestart", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> PauseVeilingAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Running)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is niet running.");

        veiling.Status = VeilingStatus.Paused;
        AddAuditEntry(veiling, "Veiling gepauzeerd", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ResumeVeilingAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is niet gepauzeerd.");

        veiling.Status = VeilingStatus.Running;
        AddAuditEntry(veiling, "Veiling hervat", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> StopVeilingAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is al beëindigd.");

        CloseCurrentIfActive(veiling);

        veiling.CurrentVeilingProductId = null;
        veiling.Status = VeilingStatus.Finished;
        veiling.EindTijdUtc ??= DateTime.UtcNow;

        AddAuditEntry(veiling, "Veiling gestopt", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ActivateNextProductAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.");

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
            await tx.CommitAsync();

            var dashFinal = await BuildDashboardResultAsync(veilingId);
            if (dashFinal.Success) await BroadcastDashboardAsync(veilingId, dashFinal.Value!);
            return dashFinal;
        }

        next.Status = VeilingProductStatus.Active;
        next.ActivatedAtUtc = DateTime.UtcNow;

        veiling.CurrentVeilingProductId = next.Id;
        veiling.Status = VeilingStatus.Running;

        AddAuditEntry(veiling, "Volgend product geactiveerd", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> CloseCurrentProductAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = await LoadVeilingForMutationAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");

        if (veiling.Status == VeilingStatus.Paused)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is gepauzeerd.");

        if (veiling.Status == VeilingStatus.Finished)
            return Result<VMVeilingDashboardDto>.Fail("Veiling is beëindigd.");

        if (veiling.CurrentVeilingProductId == null)
            return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

        var current = veiling.VeilingProducten.FirstOrDefault(p => p.Id == veiling.CurrentVeilingProductId);
        if (current == null || current.Status != VeilingProductStatus.Active)
            return Result<VMVeilingDashboardDto>.Fail("Geen actief product.");

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
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ResetVeilingAsync(int veilingId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

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

        AddAuditEntry(veiling, "Veiling gereset", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> ReorderQueueAsync(int veilingId, VMReorderQueueRequest request, int actorGebruikerId)
    {
        if (request.OrderedVeilingProductIds == null || request.OrderedVeilingProductIds.Length == 0)
            return Result<VMVeilingDashboardDto>.Fail("Geen ids meegegeven.");

        await using var tx = await _db.Database.BeginTransactionAsync();

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
            queued.First(x => x.Id == id).Volgorde = volgorde++;

        AddAuditEntry(veiling, "Queue volgorde aangepast", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<VMVeilingDashboardDto>> SkipProductAsync(int veilingId, int veilingProductId, int actorGebruikerId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

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

            AddAuditEntry(veiling, "Huidig product geskipt (gesloten)", actorGebruikerId);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var dashboardAfter = await ActivateNextProductAsync(veilingId, actorGebruikerId);
            if (dashboardAfter.Success) await BroadcastDashboardAsync(veilingId, dashboardAfter.Value!);
            return dashboardAfter;
        }

        if (product.Status != VeilingProductStatus.Queued)
            return Result<VMVeilingDashboardDto>.Fail("Alleen queued producten kun je skippen.");

        product.Volgorde = maxVolgorde + 1;

        AddAuditEntry(veiling, "Product geskipt (naar achter gezet)", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var dashboard = await BuildDashboardResultAsync(veilingId);
        if (dashboard.Success) await BroadcastDashboardAsync(veilingId, dashboard.Value!);
        return dashboard;
    }

    public async Task<Result<List<string>>> GetVeildagenAsync()
    {
        var dagen = await _db.Aanmeldingen
            .AsNoTracking()
            .Select(a => a.LeverDatum.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync();

        return Result<List<string>>.Ok(dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList());
    }

    public async Task<Result<List<VeilingPlanningAanmeldingDto>>> GetAanmeldingenAsync(string leverdatum)
    {
        if (!TryParseDateOnly(leverdatum, out var parsedDatum))
            return Result<List<VeilingPlanningAanmeldingDto>>.Fail("Leverdatum ongeldig (yyyy-MM-dd)");

        var date = parsedDatum.ToDateTime(TimeOnly.MinValue).Date;

        var items = await _db.Aanmeldingen
            .AsNoTracking()
            .Include(a => a.Aanvoerder)
            .Where(a => a.LeverDatum.Date == date && a.VeilingProductId == null)
            .Select(a => new VeilingPlanningAanmeldingDto
            {
                Id = a.Id,
                Soort = a.Soort,
                Hoeveelheid = a.Hoeveelheid,
                MinimumPrijs = a.MinimumPrijs,
                AanvoerderNaam = a.Aanvoerder!.Naam,
                LeverDatum = a.LeverDatum
            })
            .ToListAsync();

        return Result<List<VeilingPlanningAanmeldingDto>>.Ok(items);
    }

    public async Task<Result<int>> PlanVeilingAsync(PlanVeilingRequestDto dto, int actorGebruikerId)
    {
        if (!TryParseDateOnly(dto.Leverdatum, out var leverdatum))
            return Result<int>.Fail("Leverdatum ongeldig (yyyy-MM-dd)");

        if (!TryParseDateOnly(dto.Veildatum, out var veildatum))
            return Result<int>.Fail("Veildatum ongeldig (yyyy-MM-dd)");

        if (!TimeSpan.TryParse(dto.StartTijd, CultureInfo.InvariantCulture, out var startTijd))
            return Result<int>.Fail("Starttijd ongeldig (HH:mm)");

        if (dto.AanmeldingIds == null || dto.AanmeldingIds.Count == 0)
            return Result<int>.Fail("Geen aanmelding ids meegegeven.");

        await using var tx = await _db.Database.BeginTransactionAsync();

        var veildatumDate = veildatum.ToDateTime(TimeOnly.MinValue).Date;

        var veiling = await _db.Veilingen
            .Include(v => v.VeilingProducten)
            .FirstOrDefaultAsync(v => v.Status == VeilingStatus.Gepland && v.Datum.Date == veildatumDate);

        if (veiling == null)
        {
            veiling = new VeilingEntity
            {
                Datum = veildatumDate,
                StartTijd = startTijd,
                Status = VeilingStatus.Gepland,
                VeilingProducten = new List<VeilingProduct>(),
                AuditEntries = new List<AuditEntry>()
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();
        }
        else
        {
            if (veiling.StartTijd != startTijd)
                veiling.StartTijd = startTijd;
        }

        var producten = veiling.VeilingProducten ?? new List<VeilingProduct>();
        var bestaandeAanmeldingen = producten.Select(p => p.AanmeldingId).ToHashSet();

        var uniqueIncoming = dto.AanmeldingIds.Distinct().ToList();
        var dubbele = uniqueIncoming.Where(id => bestaandeAanmeldingen.Contains(id)).ToList();
        if (dubbele.Count > 0)
            return Result<int>.Fail("Geselecteerde producten zijn al aangemeld voor de veiling.");

        var aanmeldingen = await _db.Aanmeldingen
            .Where(a => uniqueIncoming.Contains(a.Id))
            .ToListAsync();

        var missing = uniqueIncoming.Except(aanmeldingen.Select(a => a.Id)).ToList();
        if (missing.Count > 0)
            return Result<int>.Fail("Eén of meer aanmeldingen bestaan niet.");

        var alreadyPlanned = aanmeldingen.Where(a => a.VeilingProductId != null).Select(a => a.Id).ToList();
        if (alreadyPlanned.Count > 0)
            return Result<int>.Fail("Eén of meer aanmeldingen zijn al gepland.");

        var leverdatumDate = leverdatum.ToDateTime(TimeOnly.MinValue).Date;

        var volgorde = producten.Any() ? producten.Max(p => p.Volgorde) + 1 : 1;

        foreach (var a in aanmeldingen)
        {
            if (a.LeverDatum.Date != leverdatumDate)
                return Result<int>.Fail("Eén of meer aanmeldingen horen niet bij de gekozen leverdatum.");

            var vp = new VeilingProduct
            {
                VeilingId = veiling.Id,
                AanmeldingId = a.Id,
                StartPrijs = a.MinimumPrijs,
                HuidigePrijs = a.MinimumPrijs,
                Volgorde = volgorde++
            };

            a.VeilingProduct = vp;
            _db.VeilingProducten.Add(vp);
        }

        AddAuditEntry(veiling, "Veiling gepland", actorGebruikerId);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<int>.Ok(veiling.Id);
    }

    public async Task<Result<List<GeplandeVeilingListItemDto>>> GetGeplandeAsync()
    {
        var veilingen = await _db.Veilingen
            .AsNoTracking()
            .Where(v => v.Status == VeilingStatus.Gepland)
            .ToListAsync();

        var productCounts = await _db.VeilingProducten
            .AsNoTracking()
            .GroupBy(p => p.VeilingId)
            .Select(g => new { VeilingId = g.Key, Aantal = g.Count() })
            .ToListAsync();

        var result = veilingen
            .OrderBy((VeilingEntity v) => v.Datum)
            .ThenBy((VeilingEntity v) => v.StartTijd)
            .Select(v =>
            {
                var aantal = productCounts.FirstOrDefault(x => x.VeilingId == v.Id)?.Aantal ?? 0;

                return new GeplandeVeilingListItemDto
                {
                    Id = v.Id,
                    Veildatum = v.Datum.ToString("yyyy-MM-dd"),
                    StartTijd = v.StartTijd.ToString(@"hh\:mm"),
                    AantalProducten = aantal
                };
            })
            .ToList();

        return Result<List<GeplandeVeilingListItemDto>>.Ok(result);
    }

    public async Task<Result<GeplandeVeilingListItemDto?>> GetVolgendeGeplandeAsync()
    {
        var veilingen = await _db.Veilingen
            .AsNoTracking()
            .Where(v => v.Status == VeilingStatus.Gepland)
            .ToListAsync();

        var volgende = veilingen
            .OrderBy((VeilingEntity v) => v.Datum)
            .ThenBy((VeilingEntity v) => v.StartTijd)
            .FirstOrDefault();

        if (volgende == null)
            return Result<GeplandeVeilingListItemDto?>.Ok(null);

        var aantal = await _db.VeilingProducten
            .AsNoTracking()
            .CountAsync(p => p.VeilingId == volgende.Id);

        return Result<GeplandeVeilingListItemDto?>.Ok(new GeplandeVeilingListItemDto
        {
            Id = volgende.Id,
            Veildatum = volgende.Datum.ToString("yyyy-MM-dd"),
            StartTijd = volgende.StartTijd.ToString(@"hh\:mm"),
            AantalProducten = aantal
        });
    }

    private async Task<Result<VMVeilingDashboardDto>> BuildDashboardResultAsync(int veilingId)
    {
        var veiling = await LoadDashboardVeilingAsync(veilingId);
        if (veiling == null) return Result<VMVeilingDashboardDto>.Fail("Veiling niet gevonden.");
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
            current.Status = VeilingProductStatus.Sold;
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

    private static bool TryParseDateOnly(string input, out DateOnly date)
    {
        return DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }
}
