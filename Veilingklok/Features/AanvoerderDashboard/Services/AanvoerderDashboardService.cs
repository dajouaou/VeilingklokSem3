using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
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
            var a = await _db.Aanvoerders
                .SingleOrDefaultAsync(x => x.GebruikerId == gebruikerId);

            if (a == null)
                throw new ArgumentException("Geen aanvoerder-profiel gevonden.");

            return a;
        }

        public async Task<AanmeldingListItemDto> CreateAanmeldingAsync(int gebruikerId, AanmeldingCreateDto dto)
        {
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            // ⛔️ Belangrijk: controleer of deze veildatum bestaat
            var bestaat = await _db.Veilingen
                .AnyAsync(v => v.StartTijd.Date == dto.Veildatum.Date);

            if (!bestaat)
                throw new ArgumentException(
                    "Deze veildatum bestaat niet of is niet gestart door de veilingmeester."
                );

            var entity = new Aanmelding
            {
                AanvoerderId = aanvoerder.Id,
                Soort = dto.Soort,
                PotmaatOfSteellengte = dto.PotmaatOfSteellengte,
                Hoeveelheid = dto.Hoeveelheid,
                MinimumPrijs = dto.MinimumPrijs,
                KlokLocatie = dto.KlokLocatie,
                Veildatum = dto.Veildatum.Date,
                FotoUrl = dto.FotoUrl
            };

            _db.Aanmeldingen.Add(entity);
            await _db.SaveChangesAsync();

            return Map(entity, isVerkocht: false, verkoopPrijs: null, koperNaam: null);
        }

        public async Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(
            int gebruikerId, DateTime? veildatum)
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
                decimal? verkoopPrijs = verkocht ? a.VeilingProduct!.HuidigePrijs : null;
                string? koperNaam = verkocht ? a.VeilingProduct!.Koper?.Naam : null;

                return Map(a, verkocht, verkoopPrijs, koperNaam);

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

            decimal opbrengst = verkocht.Sum(v =>
                v.VeilingProduct!.HuidigePrijs * v.Hoeveelheid);

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
                PotmaatOfSteellengte = a.PotmaatOfSteellengte,
                Hoeveelheid = a.Hoeveelheid,
                MinimumPrijs = a.MinimumPrijs,
                KlokLocatie = a.KlokLocatie.ToString(),
                Veildatum = a.Veildatum,
                FotoUrl = a.FotoUrl,

                IsVerkocht = isVerkocht,
                VerkoopPrijs = verkoopPrijs,
                KoperNaam = koperNaam,
                TotaleOpbrengst = isVerkocht
                    ? verkoopPrijs * a.Hoeveelheid
                    : null
            };
        }
    }
}
