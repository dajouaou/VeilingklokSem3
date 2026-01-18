using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;
using Veilingklok.Infrastructure.Time;

namespace Veilingklok.Features.Veiling.Services
{
    // Service met de businesslogica rondom veilingen (starten, pauzeren, bieden, details ophalen).
    // Deze class wordt aangeroepen door controllers en background services.
    public class VeilingService : IVeilingService
    {
        // DbContext voor database-acties (EF Core)
        private readonly MyContext _db;

        // Constructor injecteert de DbContext
        public VeilingService(MyContext db)
        {
            _db = db;
        }

        // Zoekt een veiling die momenteel gestart of gepauzeerd is en geeft het detail-overzicht terug.
        // Returnt null als er geen actieve veiling is.
        public async Task<VeilingOverzichtDto?> GetActieveVeilingAsync()
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .FirstOrDefaultAsync(v =>
                    v.Status == VeilingStatus.Gestart ||
                    v.Status == VeilingStatus.Gepauzeerd
                );

            if (v == null) return null;

            // Hergebruik dezelfde detail-opbouw voor consistent DTO resultaat
            return await GetDetailsAsync(v.Id);
        }

        // Start een bestaande geplande veiling (status: Gepland -> Gestart).
        // Zet het eerste product actief en initialiseert prijs/timer-instellingen.
        public async Task<VeilingOverzichtDto> StartGeplandeVeilingAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .SingleOrDefaultAsync(v => v.Id == veilingId);

            // Validatie: veiling moet bestaan en gepland zijn
            if (v == null) throw new ArgumentException("Veiling bestaat niet.");
            if (v.Status != VeilingStatus.Gepland) throw new ArgumentException("Veiling is niet gepland.");

            // Startmoment bepalen (datum + starttijd)
            var geplandeStart = v.Datum.Date + v.StartTijd;

            // NL tijd gebruiken (cloud/azure issues met lokale server-tijd vermijden)
            if (NlTime.Now() < geplandeStart)
                throw new ArgumentException($"Deze veiling kan pas gestart worden op {geplandeStart:yyyy-MM-dd HH:mm}");

            // Eerste product bepalen op basis van volgorde
            var first = v.Producten
                .OrderBy(p => p.Volgorde)
                .FirstOrDefault()
                ?? throw new ArgumentException("Geen producten in veiling.");

            // Veiling starten en eerste product activeren
            v.Status = VeilingStatus.Gestart;
            v.HuidigProductId = first.Id;

            first.IsActief = true;
            first.LaatstePrijsUpdateUtc = DateTime.UtcNow;

            // Veiligheidsnet: maximum/minimum prijs initialiseren als die ontbreekt
            if (first.MaximumPrijs <= 0)
            {
                first.MinimumPrijs = first.MinimumPrijs <= 0
                    ? (first.Aanmelding?.MinimumPrijs ?? 0)
                    : first.MinimumPrijs;

                first.MaximumPrijs = first.MinimumPrijs + 5m;
            }

            // Veiligheidsnet: daling instellen als die ontbreekt
            if (first.DalingPerSeconde <= 0) first.DalingPerSeconde = 0.10m;

            // Veiligheidsnet: resterende hoeveelheid initialiseren
            if (first.ResterendeHoeveelheid <= 0)
                first.ResterendeHoeveelheid = first.Aanmelding?.Hoeveelheid ?? 0;

            // Startprijs op maximumprijs zetten
            first.HuidigePrijs = first.MaximumPrijs;

            await _db.SaveChangesAsync();

            // DTO-overzicht teruggeven
            return await GetDetailsAsync(v.Id);
        }

        // Maakt een nieuwe geplande veiling aan op basis van aanmeldingen voor een leverdatum.
        // Zet producten klaar in de juiste volgorde met initiele prijs-instellingen.
        public async Task<VeilingOverzichtDto> StartVeilingAsync(DateTime veildatum, DateTime leverdatum, TimeSpan? startTijd = null)
        {
            // Haal alle aanmeldingen op die nog niet gekoppeld zijn aan een veilingproduct
            var aanmeldingen = await _db.Aanmeldingen
                .Where(a => a.LeverDatum.Date == leverdatum.Date && a.VeilingProductId == null)
                .OrderBy(a => a.Soort)
                .ToListAsync();

            // Zonder aanmeldingen valt er niets te veilen
            if (!aanmeldingen.Any())
                throw new ArgumentException("Geen aanmeldingen voor deze leverdatum.");

            // Default starttijd = 09:00 als die niet is meegegeven
            var tijd = startTijd ?? new TimeSpan(9, 0, 0);

            // Nieuwe veiling aanmaken (status Gepland)
            var veiling = new VeilingEntity
            {
                Datum = veildatum.Date,
                StartTijd = tijd,
                Status = VeilingStatus.Gepland
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();

            // Producten aanmaken in volgorde
            int volgorde = 1;

            foreach (var a in aanmeldingen)
            {
                // Simpele initiele prijslogica: max = min + 5, daling standaard 0.10
                var min = a.MinimumPrijs;
                var max = min + 5m;
                var daling = 0.10m;

                var vp = new VeilingProduct
                {
                    VeilingId = veiling.Id,
                    AanmeldingId = a.Id,

                    MinimumPrijs = min,
                    MaximumPrijs = max,
                    HuidigePrijs = max,

                    DalingPerSeconde = daling,
                    ResterendeHoeveelheid = a.Hoeveelheid,

                    Volgorde = volgorde++,
                    IsActief = false,
                    IsVerkocht = false,
                    IsDoorgedraaid = false
                };

                // Koppelingen opslaan (VP + terug-link in Aanmelding)
                _db.VeilingProducten.Add(vp);
                a.VeilingProduct = vp;
            }

            await _db.SaveChangesAsync();

            // Detail-overzicht teruggeven (nog niet gestart, maar wel producten aanwezig)
            return await GetDetailsAsync(veiling.Id);
        }

        // Bouwt het DTO-overzicht voor een veiling:
        // - status info
        // - huidig product (als aanwezig)
        // - wachtrij (alle producten die nog moeten komen)
        public async Task<VeilingOverzichtDto> GetDetailsAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                        .ThenInclude(a => a.Aanvoerder)
                .SingleAsync(v => v.Id == veilingId);

            // Basis informatie van de veiling in DTO
            var dto = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten
            };

            // Huidig product bepalen aan de hand van HuidigProductId
            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);

            // Als het product bestaat + aanmelding bestaat: map naar DTO
            if (hp != null && hp.Aanmelding != null)
            {
                dto.HuidigProduct = new HuidigProductDto
                {
                    VeilingProductId = hp.Id,
                    Soort = hp.Aanmelding.Soort,
                    FotoUrl = hp.Aanmelding.FotoUrl,

                    MaximumPrijs = hp.MaximumPrijs,
                    MinimumPrijs = hp.MinimumPrijs,
                    HuidigePrijs = hp.HuidigePrijs,

                    DalingPerSeconde = hp.DalingPerSeconde,
                    ResterendeHoeveelheid = hp.ResterendeHoeveelheid,

                    IsActief = hp.IsActief,
                    IsVerkocht = hp.IsVerkocht,
                    IsDoorgedraaid = hp.IsDoorgedraaid,

                    AanvoerderId = hp.Aanmelding.AanvoerderId,
                    AanvoerderNaam = hp.Aanmelding.Aanvoerder?.Naam ?? ""
                };
            }

            // Wachtrij: alles wat nog niet actief/verkocht/doorgedraaid is
            dto.Wachtrij = v.Producten
                .Where(p => !p.IsActief && !p.IsVerkocht && !p.IsDoorgedraaid)
                .OrderBy(p => p.Volgorde)
                .Select(p => new WachtrijItemDto
                {
                    VeilingProductId = p.Id,
                    Volgorde = p.Volgorde,
                    Soort = p.Aanmelding!.Soort,
                    FotoUrl = p.Aanmelding!.FotoUrl,

                    MaximumPrijs = p.MaximumPrijs,
                    MinimumPrijs = p.MinimumPrijs,
                    ResterendeHoeveelheid = p.ResterendeHoeveelheid,

                    AanvoerderId = p.Aanmelding!.AanvoerderId,
                    AanvoerderNaam = p.Aanmelding!.Aanvoerder != null ? p.Aanmelding!.Aanvoerder!.Naam : ""
                })
                .ToList();

            return dto;
        }

        // Pauzeert een actieve veiling (alleen toegestaan wanneer status Gestart is)
        public async Task PauseAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            if (v.Status != VeilingStatus.Gestart)
                throw new ArgumentException("Veiling is niet actief.");

            v.Status = VeilingStatus.Gepauzeerd;
            await _db.SaveChangesAsync();
        }

        // Hervat een gepauzeerde veiling en reset de prijs-timer van het huidige product
        public async Task ResumeAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(x => x.Producten)
                .FirstOrDefaultAsync(x => x.Id == veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            if (v.Status != VeilingStatus.Gepauzeerd)
                throw new ArgumentException("Veiling is niet gepauzeerd.");

            v.Status = VeilingStatus.Gestart;

            // Timer resetten zodat prijs niet direct hard daalt na resume
            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
            if (hp != null) hp.LaatstePrijsUpdateUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        // Stopt een veiling (status naar Afgesloten) en zet afsluitmoment in UTC
        public async Task StopAsync(int veilingId)
        {
            var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
            if (veiling == null) throw new Exception("Veiling niet gevonden");

            // Alleen aanpassen als de veiling nog niet afgesloten is
            if (veiling.Status != VeilingStatus.Afgesloten)
            {
                veiling.Status = VeilingStatus.Afgesloten;
                veiling.AfgeslotenOpUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        // Plaatst een bod voor een koper, maakt direct een transactie aan
        // en werkt de resterende hoeveelheid bij.
        public async Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperGebruikerId)
        {
            // 1) Koper-profiel opzoeken (voor Kopers.Id)
            var koperProfiel = await _db.Kopers
                .AsNoTracking()
                .SingleOrDefaultAsync(k => k.GebruikerId == koperGebruikerId);

            if (koperProfiel == null)
                throw new ArgumentException("Je hebt geen koper-profiel.");

            var koperId = koperProfiel.Id;

            // 2) Veiling inclusief producten ophalen
            var veiling = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .SingleAsync(v => v.Id == veilingId);

            // 3) Alleen bieden tijdens een gestartte veiling
            if (veiling.Status != VeilingStatus.Gestart)
                throw new ArgumentException("Veiling is niet gestart.");

            // 4) Product ophalen en basisvalidaties
            var product = veiling.Producten.Single(p => p.Id == dto.VeilingProductId);

            if (!product.IsActief) throw new ArgumentException("Product is niet actief.");
            if (product.IsVerkocht || product.IsDoorgedraaid) throw new ArgumentException("Product is niet meer beschikbaar.");

            // 5) Aantal bepalen (0 of kleiner betekent: alles kopen)
            var koopAantal = dto.Aantal <= 0 ? product.ResterendeHoeveelheid : dto.Aantal;
            if (koopAantal <= 0 || koopAantal > product.ResterendeHoeveelheid)
                throw new ArgumentException("Ongeldig aantal.");

            // 6) Prijs bepalen (0 betekent: huidige klokprijs)
            if (dto.Prijs <= 0) dto.Prijs = product.HuidigePrijs;
            product.HuidigePrijs = dto.Prijs;

            // 7) Bod opslaan
            var bod = new Bod
            {
                VeilingId = veilingId,
                VeilingProductId = product.Id,
                KoperId = koperId,
                Prijs = dto.Prijs,
                Aantal = koopAantal
            };
            _db.Biedingen.Add(bod);

            // 8) Transactie opslaan (voor historie/administratie)
            var transactie = new Transactie
            {
                VeilingId = veilingId,
                VeilingProductId = product.Id,
                KoperId = koperGebruikerId,
                Aantal = koopAantal,
                Prijs = dto.Prijs,
                Tijdstip = DateTime.UtcNow
            };
            _db.Transacties.Add(transactie);

            // 9) Voorraad bijwerken + koper koppelen aan product
            product.ResterendeHoeveelheid -= koopAantal;
            product.KoperId = koperId;

            if (product.ResterendeHoeveelheid > 0)
            {
                // Restpartij: klok opnieuw laten starten op maximumprijs
                product.HuidigePrijs = product.MaximumPrijs;
                product.LaatstePrijsUpdateUtc = DateTime.UtcNow;
            }
            else
            {
                // Partij volledig verkocht: markeer en ga door naar volgende
                product.IsVerkocht = true;
                product.IsActief = false;

                var volgende = veiling.Producten
                    .Where(p => !p.IsVerkocht && !p.IsDoorgedraaid && !p.IsActief)
                    .OrderBy(p => p.Volgorde)
                    .FirstOrDefault();

                if (volgende != null)
                {
                    // Volgende product activeren + initiele waarden instellen
                    volgende.IsActief = true;
                    if (volgende.MinimumPrijs <= 0) volgende.MinimumPrijs = volgende.Aanmelding?.MinimumPrijs ?? 0m;
                    if (volgende.MaximumPrijs <= 0) volgende.MaximumPrijs = volgende.MinimumPrijs + 5m;
                    if (volgende.DalingPerSeconde <= 0) volgende.DalingPerSeconde = 0.10m;
                    if (volgende.ResterendeHoeveelheid <= 0) volgende.ResterendeHoeveelheid = volgende.Aanmelding?.Hoeveelheid ?? 0;

                    volgende.HuidigePrijs = volgende.MaximumPrijs;
                    volgende.LaatstePrijsUpdateUtc = DateTime.UtcNow;

                    // Veiling wijst nu naar het volgende product
                    veiling.HuidigProductId = volgende.Id;
                }
                else
                {
                    // Geen producten meer: veiling afsluiten
                    veiling.Status = VeilingStatus.Afgesloten;
                    veiling.AfgeslotenOpUtc ??= DateTime.UtcNow;
                    veiling.HuidigProductId = null;
                }
            }

            await _db.SaveChangesAsync();

            // 10) Kopernaam teruggeven voor UI (komt uit Gebruiker record)
            var koperUser = await _db.Gebruikers.AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == koperGebruikerId);

            return new BodDto
            {
                Id = bod.Id,
                Prijs = bod.Prijs,
                Tijdstip = bod.Tijdstip,
                KoperNaam = koperUser != null ? $"{koperUser.Voornaam} {koperUser.Achternaam}" : null
            };
        }

        // Geeft alle unieke veildagen terug als strings (yyyy-MM-dd)
        public async Task<List<string>> GetVeilingDagenAsync()
        {
            var dates = await _db.Veilingen
                .Select(v => v.Datum)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return dates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }
    }
}
