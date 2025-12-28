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
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services;

public sealed class VeilingPlanningService : IVeilingPlanningService
{
    private const int DefaultDurationSeconds = 20;
    private readonly MyContext _db;

    public VeilingPlanningService(MyContext db)
    {
        _db = db;
    }

    public async Task<Result<int>> CreateVeilingFromLeverdatumAsync(CreateVeilingFromLeverdatumDto dto, int actorGebruikerId)
    {
        if (!DateTime.TryParse(dto.Leverdatum, out var leverDatum))
            return Result<int>.Fail("Leverdatum is ongeldig.");

        var aanmeldingen = await _db.Aanmeldingen
            .Where(a => a.LeverDatum.Date == leverDatum.Date && a.VeilingProductId == null)
            .OrderBy(a => a.Soort)
            .ThenBy(a => a.Id)
            .ToListAsync();

        if (aanmeldingen.Count == 0)
            return Result<int>.Fail("Geen aanmeldingen voor deze leverdatum.");

        var now = DateTime.UtcNow;

        await using var tx = await _db.Database.BeginTransactionAsync();

        var veiling = new Core.Entities.Veiling
        {
            Datum = dto.Veildatum.Date,
            StartTijd = dto.StartTijd ?? new TimeSpan(9, 0, 0),
            Status = VeilingStatus.Scheduled
        };

        _db.Veilingen.Add(veiling);
        await _db.SaveChangesAsync();

        var volgorde = 1;
        var created = new List<(Aanmelding A, VeilingProduct VP)>();

        foreach (var a in aanmeldingen)
        {
            var startPrijs = a.MinimumPrijs;

            var vp = new VeilingProduct
            {
                VeilingId = veiling.Id,
                AanmeldingId = a.Id,
                StartPrijs = startPrijs,
                HuidigePrijs = startPrijs,
                MinimumPrijs = a.MinimumPrijs,
                DurationSeconds = DefaultDurationSeconds,
                Status = VeilingProductStatus.Queued,
                Volgorde = volgorde++,
                Hoeveelheid = a.Hoeveelheid
            };

            _db.VeilingProducten.Add(vp);
            created.Add((a, vp));
        }

        await _db.SaveChangesAsync();

        foreach (var (a, vp) in created)
            a.VeilingProductId = vp.Id;

        _db.AuditEntries.Add(new AuditEntry
        {
            VeilingId = veiling.Id,
            ActorGebruikerId = actorGebruikerId,
            Action = $"Veiling aangemaakt vanuit leverdatum {leverDatum:yyyy-MM-dd}",
            CreatedAtUtc = now
        });

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return Result<int>.Ok(veiling.Id);
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
            veiling = new Core.Entities.Veiling
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
            .OrderBy(v => v.Datum)
            .ThenBy(v => v.StartTijd)
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
            .OrderBy(v => v.Datum)
            .ThenBy(v => v.StartTijd)
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

    public async Task<Result<VeilingDetailsDto>> GetDetailsAsync(int veilingId)
    {
        var veiling = await _db.Veilingen
            .AsNoTracking()
            .Include(v => v.VeilingProducten)
                .ThenInclude(vp => vp.Aanmelding)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        if (veiling == null)
            return Result<VeilingDetailsDto>.Fail("Veiling niet gevonden.");

        VeilingProduct? current = null;

        if (veiling.CurrentVeilingProductId is int currentId)
        {
            var candidate = veiling.VeilingProducten.FirstOrDefault(p => p.Id == currentId);
            if (candidate != null && candidate.Status == VeilingProductStatus.Active)
                current = candidate;
        }

        var queue = veiling.VeilingProducten
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

        return Result<VeilingDetailsDto>.Ok(new VeilingDetailsDto
        {
            Id = veiling.Id,
            Status = veiling.Status,
            CurrentVeilingProductId = veiling.CurrentVeilingProductId,
            CurrentProduct = current == null
                ? null
                : new HuidigProductDto
                {
                    VeilingProductId = current.Id,
                    Soort = current.Aanmelding?.Soort ?? string.Empty,
                    FotoUrl = current.Aanmelding?.FotoUrl,
                    StartPrijs = current.StartPrijs,
                    HuidigePrijs = current.HuidigePrijs,
                    Hoeveelheid = current.Aanmelding?.Hoeveelheid ?? current.Hoeveelheid,
                    IsActief = true,
                    IsVerkocht = false
                },
            Queue = queue
        });
    }

    private static void AddAuditEntry(Core.Entities.Veiling veiling, string action, int actorGebruikerId)
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
