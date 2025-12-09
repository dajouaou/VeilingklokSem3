using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Infrastructure.Database;


namespace Veilingklok.Features.AanvoerderDashboard.Services
{
    public class AanvoerderDashboardService : IAanvoerderDashboardService
    {
        private readonly MyContext _db;

        public AanvoerderDashboardService(MyContext db)
        {
            _db = db;
        }

        private async Task<Aanvoerder> GetAanvoerderForGebruikerAsync(int gebruikerId)
        {
            var a = await _db.Aanvoerders.SingleOrDefaultAsync(x => x.GebruikerId == gebruikerId);
            if (a == null)
                throw new ArgumentException("Geen aanvoerder-profiel gevonden.");
            return a;
        }

        private static readonly HashSet<DateTime> Feestdagen = new()
        {
            new DateTime(2025, 1, 1),
            new DateTime(2025, 4, 18),
            new DateTime(2025, 4, 20),
            new DateTime(2025, 4, 21),
            new DateTime(2025, 5, 29),
            new DateTime(2025, 6, 8),
            new DateTime(2025, 6, 9),
            new DateTime(2025, 12, 25),
            new DateTime(2025, 12, 26)
        };
        public async Task<AanmeldingListItemDto> CreateAanmeldingAsync(
            int gebruikerId,
            AanmeldingCreateDto dto,
            string? fotoUrl)
        {
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);
            var datum = dto.Veildatum.Date;

            if (datum.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                throw new ArgumentException("Zaterdag en zondag zijn geen geldige veildagen.");

            if (Feestdagen.Contains(datum))
                throw new ArgumentException("Deze dag is een feestdag en kan niet gekozen worden.");

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
                Veildatum = datum,
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
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

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
            entity.Veildatum = dto.Veildatum.Date;

            if (fotoUrl != null)
                entity.FotoUrl = fotoUrl;

            await _db.SaveChangesAsync();

            return Map(entity, false, null, null);
        }

        public async Task DeleteAanmeldingAsync(int gebruikerId, int id)
        {
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            var entity = await _db.Aanmeldingen
                .FirstOrDefaultAsync(a => a.Id == id && a.AanvoerderId == aanvoerder.Id);

            if (entity == null)
                throw new ArgumentException("Aanmelding niet gevonden.");

            _db.Aanmeldingen.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(int gebruikerId, DateTime? veildatum)
        {
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            var query = _db.Aanmeldingen
                .Include(a => a.VeilingProduct)
                .ThenInclude(vp => vp.Koper)
                .Where(a => a.AanvoerderId == aanvoerder.Id);

            if (veildatum.HasValue)
                query = query.Where(a => a.Veildatum == veildatum.Value.Date);

            var list = await query.ToListAsync();

            return list.Select(a =>
            {
                bool verkocht = a.VeilingProduct != null && a.VeilingProduct.IsVerkocht;
                return Map(
                    a,
                    verkocht,
                    verkocht ? a.VeilingProduct!.HuidigePrijs : null,
                    verkocht ? a.VeilingProduct!.Koper?.Naam : null
                );
            }).ToList();
        }

        public async Task<AanvoerderStatsDto> GetStatsAsync(int gebruikerId, DateTime? veildatum)
        {
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            var query = _db.Aanmeldingen
                .Include(a => a.VeilingProduct)
                .Where(a => a.AanvoerderId == aanvoerder.Id);

            if (veildatum.HasValue)
                query = query.Where(a => a.Veildatum == veildatum.Value.Date);

            var list = await query.ToListAsync();

            var totaal = list.Count;
            var verkocht = list.Where(a => a.VeilingProduct != null && a.VeilingProduct.IsVerkocht);

            decimal opbrengst = verkocht.Sum(v => v.VeilingProduct!.HuidigePrijs * v.Hoeveelheid);

            return new AanvoerderStatsDto
            {
                TotaalAantalAanmeldingen = totaal,
                AantalVerkocht = verkocht.Count(),
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
                Veildatum = a.Veildatum,
                FotoUrl = a.FotoUrl,
                IsVerkocht = isVerkocht,
                VerkoopPrijs = verkoopPrijs,
                KoperNaam = koperNaam,
                TotaleOpbrengst = isVerkocht ? verkoopPrijs * a.Hoeveelheid : null,
                Beschrijving = a.Beschrijving          // nieuw
            };
        }

    }
}
