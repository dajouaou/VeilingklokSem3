using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Infrastructure.Database;


namespace Veilingklok.Features.AanvoerderDashboard.Services
{
    // Service met alle businesslogica voor het aanvoerder dashboard
    public class AanvoerderDashboardService : IAanvoerderDashboardService
    {
        private readonly MyContext _db;

        // Injecteert de database context
        public AanvoerderDashboardService(MyContext db)
        {
            _db = db;
        }

        // Haalt het aanvoerder-profiel op bij een gebruiker
        private async Task<Aanvoerder> GetAanvoerderForGebruikerAsync(int gebruikerId)
        {
            var a = await _db.Aanvoerders.SingleOrDefaultAsync(x => x.GebruikerId == gebruikerId);
            if (a == null)
                throw new ArgumentException("Geen aanvoerder-profiel gevonden.");
            return a;
        }

        // Lijst met vaste feestdagen die niet gekozen mogen worden
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

        // Maakt een nieuwe aanmelding aan
        public async Task<AanmeldingListItemDto> CreateAanmeldingAsync(
            int gebruikerId,
            AanmeldingCreateDto dto,
            string? fotoUrl)
        {
            // Haalt de juiste aanvoerder op
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);
            var datum = dto.LeverDatum.Date;

            // Staat testmodus toe als leverdatum vandaag is
            var isTestMode = datum == DateTime.Today;

            // Blokkeert weekenden
            if (!isTestMode && datum.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                throw new ArgumentException("Zaterdag en zondag zijn geen geldige veildagen.");

            // Blokkeert feestdagen
            if (!isTestMode && Feestdagen.Contains(datum))
                throw new ArgumentException("Deze dag is een feestdag en kan niet gekozen worden.");

            // Checkt of potmaat of steellengte is ingevuld
            if (string.IsNullOrWhiteSpace(dto.Potmaat) &&
                string.IsNullOrWhiteSpace(dto.Steellengte))
                throw new ArgumentException("Vul potmaat of steellengte in.");

            // Bouwt een nieuwe aanmelding entity
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

            // Slaat de aanmelding op
            _db.Aanmeldingen.Add(entity);
            await _db.SaveChangesAsync();

            // Geeft de aangemaakte aanmelding terug als DTO
            return Map(entity, false, null, null);
        }

        // Werkt een bestaande aanmelding bij
        public async Task<AanmeldingListItemDto> UpdateAanmeldingAsync(
            int gebruikerId,
            int id,
            AanmeldingUpdateDto dto,
            string? fotoUrl)
        {
            // Haalt de juiste aanvoerder op
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            // Zoekt de aanmelding die bij deze aanvoerder hoort
            var entity = await _db.Aanmeldingen
                .FirstOrDefaultAsync(a => a.Id == id && a.AanvoerderId == aanvoerder.Id);

            if (entity == null)
                throw new ArgumentException("Aanmelding niet gevonden.");

            // Past de velden aan
            entity.Soort = dto.Soort;
            entity.Potmaat = dto.Potmaat;
            entity.Steellengte = dto.Steellengte;
            entity.Hoeveelheid = dto.Hoeveelheid;
            entity.MinimumPrijs = dto.MinimumPrijs;
            entity.KlokLocatie = dto.KlokLocatie;
            entity.LeverDatum = dto.LeverDatum.Date;
            entity.Beschrijving = dto.Beschrijving;

            // Zet nieuwe foto als die is meegestuurd
            if (fotoUrl != null)
                entity.FotoUrl = fotoUrl;

            // Slaat de wijziging op
            await _db.SaveChangesAsync();

            // Geeft de bijgewerkte aanmelding terug
            return Map(entity, false, null, null);
        }

        // Verwijdert een aanmelding
        public async Task DeleteAanmeldingAsync(int gebruikerId, int id)
        {
            // Haalt de juiste aanvoerder op
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            // Zoekt de aanmelding zonder tracking
            var entity = await _db.Aanmeldingen
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.AanvoerderId == aanvoerder.Id);

            if (entity == null)
                throw new ArgumentException("Aanmelding niet gevonden.");

            // Blokkeert verwijderen als de aanmelding al ingepland is
            if (entity.VeilingProductId != null)
                throw new ArgumentException("Kan niet verwijderen: deze aanmelding zit al in een geplande veiling.");

            // Verwijdert de aanmelding
            _db.Aanmeldingen.Remove(new Aanmelding { Id = id });
            await _db.SaveChangesAsync();
        }



        // Haalt alle aanmeldingen op voor een aanvoerder
        public async Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(int gebruikerId, DateTime? veildatum)
        {
            // Haalt de juiste aanvoerder op
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            // Bouwt de basisquery
            var query = _db.Aanmeldingen
                .Include(a => a.Aanvoerder)
                .Include(a => a.VeilingProduct)
                .ThenInclude(vp => vp.Koper)
                .Where(a => a.AanvoerderId == aanvoerder.Id);

            // Filtert op datum als die is meegegeven
            if (veildatum.HasValue)
                query = query.Where(a => a.LeverDatum == veildatum.Value.Date);

            var list = await query.ToListAsync();

            // Zet entities om naar DTO’s
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

        // Haalt statistieken op voor het dashboard
        public async Task<AanvoerderStatsDto> GetStatsAsync(int gebruikerId, DateTime? veildatum)
        {
            // Haalt de juiste aanvoerder op
            var aanvoerder = await GetAanvoerderForGebruikerAsync(gebruikerId);

            // Bouwt de basisquery
            var query = _db.Aanmeldingen
                .Include(a => a.VeilingProduct)
                .Where(a => a.AanvoerderId == aanvoerder.Id);

            // Filtert op datum als die is meegegeven
            if (veildatum.HasValue)
                query = query.Where(a => a.LeverDatum == veildatum.Value.Date);

            var list = await query.ToListAsync();

            // Berekent totalen en opbrengst
            var totaal = list.Count;
            var verkocht = list.Where(a => a.VeilingProduct != null && a.VeilingProduct.IsVerkocht);

            decimal opbrengst = verkocht.Sum(v => v.VeilingProduct!.HuidigePrijs * v.Hoeveelheid);

            // Bouwt en geeft de statistieken DTO terug
            return new AanvoerderStatsDto
            {
                TotaalAantalAanmeldingen = totaal,
                AantalVerkocht = verkocht.Count(),
                TotaleOpbrengst = opbrengst
            };
        }

        // Zet een Aanmelding entity om naar een dashboard DTO
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
                IsVerkocht = isVerkocht,
                VerkoopPrijs = verkoopPrijs,
                KoperNaam = koperNaam,
                TotaleOpbrengst = isVerkocht ? verkoopPrijs * a.Hoeveelheid : null,
                Beschrijving = a.Beschrijving,
                AanvoerderNaam = a.Aanvoerder?.Naam,

            };
        }

    }
}
