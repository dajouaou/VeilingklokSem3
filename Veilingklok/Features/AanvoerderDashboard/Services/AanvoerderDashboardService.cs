using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.AanvoerderDashboard.Services;

public sealed class AanvoerderDashboardService : IAanvoerderDashboardService
{
    private readonly MyContext _db;

    public AanvoerderDashboardService(MyContext db)
    {
        _db = db;
    }

    private async Task<Aanvoerder> GetAanvoerderAsync(int gebruikerId)
    {
        var a = await _db.Aanvoerders
            .Include(x => x.Gebruiker)
            .SingleOrDefaultAsync(x => x.GebruikerId == gebruikerId);

        if (a == null)
            throw new ArgumentException("Geen aanvoerder-profiel gevonden.");

        return a;
    }

    private static bool IsVerkocht(VeilingProduct vp)
        => vp.KoperId != null
           && vp.ClosedAtUtc != null
           && vp.Status == VeilingProductStatus.Sold;

    public async Task<AanmeldingListItemDto> CreateAanmeldingAsync(
        int gebruikerId,
        AanmeldingCreateDto dto,
        string? fotoUrl)
    {
        var aanvoerder = await GetAanvoerderAsync(gebruikerId);
        var datum = dto.LeverDatum.Date;

        var isTestMode = datum == DateTime.Today;

        if (!isTestMode && datum.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new ArgumentException("Zaterdag en zondag zijn geen geldige veildagen.");

        if (!isTestMode && Feestdagen.Contains(datum))
            throw new ArgumentException("Deze dag is een feestdag.");

        if (string.IsNullOrWhiteSpace(dto.Potmaat) &&
            string.IsNullOrWhiteSpace(dto.Steellengte))
            throw new ArgumentException("Vul potmaat of steellengte in.");

        var entity = new Aanmelding
        {
            AanvoerderId = aanvoerder.Id,
            Soort = dto.Soort,
            Potmaat = dto.Potmaat,
            Steellengte = dto.Steellengte,
            Hoeveelheid = dto.Hoeveelheid,
            MinimumPrijs = dto.MinimumPrijs,
            KlokLocatie = dto.KlokLocatie,
            LeverDatum = datum,
            FotoUrl = fotoUrl,
            Beschrijving = dto.Beschrijving
        };

        _db.Aanmeldingen.Add(entity);
        await _db.SaveChangesAsync();

        return Map(entity, false, null, null);
    }

    public async Task<AanmeldingListItemDto> UpdateAanmeldingAsync(
        int gebruikerId,
        int id,
        AanmeldingUpdateDto dto,
        string? fotoUrl)
    {
        var aanvoerder = await GetAanvoerderAsync(gebruikerId);

        var entity = await _db.Aanmeldingen
            .FirstOrDefaultAsync(a => a.Id == id && a.AanvoerderId == aanvoerder.Id);

        if (entity == null)
            throw new ArgumentException("Aanmelding niet gevonden.");

        entity.Soort = dto.Soort;
        entity.Potmaat = dto.Potmaat;
        entity.Steellengte = dto.Steellengte;
        entity.Hoeveelheid = dto.Hoeveelheid;
        entity.MinimumPrijs = dto.MinimumPrijs;
        entity.KlokLocatie = dto.KlokLocatie;
        entity.LeverDatum = dto.LeverDatum.Date;
        entity.Beschrijving = dto.Beschrijving;

        if (fotoUrl != null)
            entity.FotoUrl = fotoUrl;

        await _db.SaveChangesAsync();

        return Map(entity, false, null, null);
    }

    public async Task DeleteAanmeldingAsync(int gebruikerId, int id)
    {
        var aanvoerder = await GetAanvoerderAsync(gebruikerId);

        var entity = await _db.Aanmeldingen
            .FirstOrDefaultAsync(a => a.Id == id && a.AanvoerderId == aanvoerder.Id);

        if (entity == null)
            throw new ArgumentException("Aanmelding niet gevonden.");

        _db.Aanmeldingen.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(
        int gebruikerId,
        DateTime? veildatum)
    {
        var aanvoerder = await GetAanvoerderAsync(gebruikerId);

        var query = _db.Aanmeldingen
            .Include(a => a.Aanvoerder)
            .Include(a => a.VeilingProduct)
                .ThenInclude(vp => vp!.Koper)
            .Where(a => a.AanvoerderId == aanvoerder.Id);

        if (veildatum.HasValue)
            query = query.Where(a => a.LeverDatum == veildatum.Value.Date);

        var list = await query.ToListAsync();

        return list.Select(a =>
        {
            var vp = a.VeilingProduct;
            var verkocht = vp != null && IsVerkocht(vp);

            var verkoopPrijs = verkocht ? vp!.HuidigePrijs : (decimal?)null;
            var koperNaam = verkocht ? vp!.Koper?.Naam : null;

            return Map(a, verkocht, verkoopPrijs, koperNaam);
        }).ToList();
    }

    public async Task<AanvoerderStatsDto> GetStatsAsync(int gebruikerId, DateTime? veildatum)
    {
        var aanvoerder = await GetAanvoerderAsync(gebruikerId);

        var query = _db.Aanmeldingen
            .Include(a => a.VeilingProduct)
            .Where(a => a.AanvoerderId == aanvoerder.Id);

        if (veildatum.HasValue)
            query = query.Where(a => a.LeverDatum == veildatum.Value.Date);

        var list = await query.ToListAsync();

        var verkochte = list
            .Where(a => a.VeilingProduct != null && IsVerkocht(a.VeilingProduct))
            .ToList();

        var opbrengst = verkochte.Sum(a =>
            a.VeilingProduct!.HuidigePrijs * a.Hoeveelheid);

        return new AanvoerderStatsDto
        {
            TotaalAantalAanmeldingen = list.Count,
            AantalVerkocht = verkochte.Count,
            TotaleOpbrengst = opbrengst
        };
    }

    private static AanmeldingListItemDto Map(
        Aanmelding a,
        bool isVerkocht,
        decimal? verkoopPrijs,
        string? koperNaam)
    {
        return new AanmeldingListItemDto
        {
            Id = a.Id,
            Soort = a.Soort,
            Potmaat = a.Potmaat,
            Steellengte = a.Steellengte,
            Hoeveelheid = a.Hoeveelheid,
            MinimumPrijs = a.MinimumPrijs,
            KlokLocatie = a.KlokLocatie.ToString(),
            LeverDatum = a.LeverDatum,
            FotoUrl = a.FotoUrl,
            Beschrijving = a.Beschrijving,
            IsVerkocht = isVerkocht,
            VerkoopPrijs = verkoopPrijs,
            KoperNaam = koperNaam,
            TotaleOpbrengst =
                isVerkocht && verkoopPrijs.HasValue
                    ? verkoopPrijs.Value * a.Hoeveelheid
                    : null,
            AanvoerderNaam = a.Aanvoerder?.Naam
        };
    }

    private static readonly HashSet<DateTime> Feestdagen = new()
    {
        new(2025, 1, 1),
        new(2025, 4, 18),
        new(2025, 4, 20),
        new(2025, 4, 21),
        new(2025, 5, 29),
        new(2025, 6, 8),
        new(2025, 6, 9),
        new(2025, 12, 25),
        new(2025, 12, 26)
    };
}
