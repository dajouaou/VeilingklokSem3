using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;


namespace Veilingklok.Features.Veiling.Services
{
    // Service met alle veiling-logica (starten, pauzeren, details, bieden)
    public class VeilingService : IVeilingService
    {
        private readonly MyContext _db;

        // Injecteert de database context
        public VeilingService(MyContext db)
        {
            _db = db;
        }

        // Zoekt de huidige actieve veiling en geeft het overzicht terug
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

            return await GetDetailsAsync(v.Id);
        }


        // Start een geplande veiling als de starttijd bereikt is en zet het eerste product actief
        public async Task<VeilingOverzichtDto> StartGeplandeVeilingAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .SingleOrDefaultAsync(v => v.Id == veilingId);

            if (v == null) throw new ArgumentException("Veiling bestaat niet.");
            if (v.Status != VeilingStatus.Gepland) throw new ArgumentException("Veiling is niet gepland.");

            var geplandeStart = v.Datum.Date + v.StartTijd;
            if (DateTime.Now < geplandeStart)
                throw new ArgumentException($"Deze veiling kan pas gestart worden op {geplandeStart:yyyy-MM-dd HH:mm}");

            var first = v.Producten
                .OrderBy(p => p.Volgorde)
                .FirstOrDefault()
                ?? throw new ArgumentException("Geen producten in veiling.");

            v.Status = VeilingStatus.Gestart;
            v.HuidigProductId = first.Id;

            // Zet het eerste product actief en reset de klok-waarden
            first.IsActief = true;
            first.LaatstePrijsUpdateUtc = DateTime.UtcNow;
            if (first.MaximumPrijs <= 0)
            {
                // Vult min/max alsnog in als die nog leeg zijn
                first.MinimumPrijs = first.MinimumPrijs <= 0 ? (first.Aanmelding?.MinimumPrijs ?? 0) : first.MinimumPrijs;
                first.MaximumPrijs = first.MinimumPrijs + 5m;
            }

            if (first.DalingPerSeconde <= 0) first.DalingPerSeconde = 0.10m;

            if (first.ResterendeHoeveelheid <= 0)
                first.ResterendeHoeveelheid = first.Aanmelding?.Hoeveelheid ?? 0;

            // Start prijs bovenaan
            first.HuidigePrijs = first.MaximumPrijs;

            await _db.SaveChangesAsync();

            return await GetDetailsAsync(v.Id);
        }

        // Maakt een geplande veiling aan op basis van aanmeldingen en zet alle veilingproducten klaar
        public async Task<VeilingOverzichtDto> StartVeilingAsync(DateTime veildatum, DateTime leverdatum, TimeSpan? startTijd = null)
        {
            var aanmeldingen = await _db.Aanmeldingen
                .Where(a => a.LeverDatum.Date == leverdatum.Date && a.VeilingProductId == null)
                .OrderBy(a => a.Soort)
                .ToListAsync();

            if (!aanmeldingen.Any())
                throw new ArgumentException("Geen aanmeldingen voor deze leverdatum.");

            var tijd = startTijd ?? new TimeSpan(9, 0, 0);

            var veiling = new VeilingEntity
            {
                Datum = veildatum.Date,
                StartTijd = tijd,
                Status = VeilingStatus.Gepland
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();

            int volgorde = 1;

            foreach (var a in aanmeldingen)
            {
                // Zet fallback prijs/tempo instellingen voor het veilingproduct
                var min = a.MinimumPrijs;
                var max = min + 5m;                // fallback
                var daling = 0.10m;              // fallback

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

                _db.VeilingProducten.Add(vp);
                a.VeilingProduct = vp;
            }

            await _db.SaveChangesAsync();
            return await GetDetailsAsync(veiling.Id);
        }

        // Haalt alle details op voor een veiling en zet het om naar een overzicht DTO
        public async Task<VeilingOverzichtDto> GetDetailsAsync(int veilingId)
        {
            var v = await _db.Veilingen
     .Include(v => v.Producten)
         .ThenInclude(p => p.Aanmelding)
             .ThenInclude(a => a.Aanvoerder)
     .SingleAsync(v => v.Id == veilingId);


            var dto = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten
            };

            // Bouwt huidig product DTO als er een actief product is
            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);

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

            // Bouwt de wachtrij met producten die nog moeten komen
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

        // Zet een veiling op pauze
        public async Task PauseAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            if (v.Status != VeilingStatus.Gestart)
                throw new ArgumentException("Veiling is niet actief.");

            v.Status = VeilingStatus.Gepauzeerd;
            await _db.SaveChangesAsync();
        }

        // Hervat een gepauzeerde veiling en reset de timer voor prijsdaling
        public async Task ResumeAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(x => x.Producten)
                .FirstOrDefaultAsync(x => x.Id == veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            if (v.Status != VeilingStatus.Gepauzeerd)
                throw new ArgumentException("Veiling is niet gepauzeerd.");

            v.Status = VeilingStatus.Gestart;

            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
            if (hp != null) hp.LaatstePrijsUpdateUtc = DateTime.UtcNow; // ✅ reset

            await _db.SaveChangesAsync();
        }


        // Sluit een veiling af en zet de afsluitdatum
        public async Task StopAsync(int veilingId)
        {
            var veiling = await _db.Veilingen.FirstOrDefaultAsync(v => v.Id == veilingId);
            if (veiling == null) throw new Exception("Veiling niet gevonden");

            if (veiling.Status != VeilingStatus.Afgesloten)
            {
                veiling.Status = VeilingStatus.Afgesloten;
                veiling.AfgeslotenOpUtc = DateTime.UtcNow;  
                await _db.SaveChangesAsync();
            }

        }


        // Plaatst een bod/koop op het huidige product en verwerkt voorraad + volgende stap in de veiling
        public async Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperGebruikerId)
        {
            // Haalt het koper-profiel op zodat je de KoperId kan koppelen
            var koperProfiel = await _db.Kopers
                .AsNoTracking()
                .SingleOrDefaultAsync(k => k.GebruikerId == koperGebruikerId);

            if (koperProfiel == null)
                throw new ArgumentException("Je hebt geen koper-profiel.");

            var koperId = koperProfiel.Id;

            // Haalt de veiling op inclusief producten en aanmelding-data
            var veiling = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .SingleAsync(v => v.Id == veilingId);

            if (veiling.Status != VeilingStatus.Gestart)
                throw new ArgumentException("Veiling is niet gestart.");

            // Zoekt het product waarop gekocht wordt
            var product = veiling.Producten.Single(p => p.Id == dto.VeilingProductId);

            if (!product.IsActief) throw new ArgumentException("Product is niet actief.");
            if (product.IsVerkocht || product.IsDoorgedraaid) throw new ArgumentException("Product is niet meer beschikbaar.");

            // Bepaalt hoeveel er gekocht wordt en valideert dat
            var koopAantal = dto.Aantal <= 0 ? product.ResterendeHoeveelheid : dto.Aantal;
            if (koopAantal <= 0 || koopAantal > product.ResterendeHoeveelheid)
                throw new ArgumentException("Ongeldig aantal.");

            // Zet prijs op huidige prijs als er geen prijs is meegestuurd
            if (dto.Prijs <= 0) dto.Prijs = product.HuidigePrijs;
            product.HuidigePrijs = dto.Prijs;

            // Slaat het bod op als record
            var bod = new Bod
            {
                VeilingId = veilingId,
                VeilingProductId = product.Id,

                // ⚠️ kies één betekenis:
                // Als Bod.KoperId een "Kopers.Id" is: zet koperId
                // Als Bod.KoperId een "Gebruikers.Id" is: zet koperGebruikerId
                KoperId = koperId,

                Prijs = dto.Prijs,
                Aantal = koopAantal
            };
            _db.Biedingen.Add(bod);

            // Slaat de transactie op als verkooprecord
            var transactie = new Transactie
            {
                VeilingId = veilingId,
                VeilingProductId = product.Id,

                // ✅ Transactie.KoperId wijst naar Gebruiker (jouw model heeft Gebruiker? Koper)
                KoperId = koperGebruikerId,

                Aantal = koopAantal,
                Prijs = dto.Prijs,
                Tijdstip = DateTime.UtcNow
            };
            _db.Transacties.Add(transactie);

            // Past voorraad aan
            product.ResterendeHoeveelheid -= koopAantal;

            // Koppelt koper aan het product
            product.KoperId = koperId; // ✅ Kopers.Id

            // Regelt wat er gebeurt na de koop (rest voorraad of product klaar)
            if (product.ResterendeHoeveelheid > 0)
            {
                product.HuidigePrijs = product.MaximumPrijs;
                product.LaatstePrijsUpdateUtc = DateTime.UtcNow; // ✅ reset timer
            }
            else
            {
                product.IsVerkocht = true;
                product.IsActief = false;

                // Zoekt het volgende product dat nog moet komen
                var volgende = veiling.Producten
                    .Where(p => !p.IsVerkocht && !p.IsDoorgedraaid && !p.IsActief)
                    .OrderBy(p => p.Volgorde)
                    .FirstOrDefault();

                if (volgende != null)
                {
                    // Activeert het volgende product en zet klok-waarden klaar
                    volgende.IsActief = true;
                    if (volgende.MinimumPrijs <= 0) volgende.MinimumPrijs = volgende.Aanmelding?.MinimumPrijs ?? 0m;
                    if (volgende.MaximumPrijs <= 0) volgende.MaximumPrijs = volgende.MinimumPrijs + 5m;
                    if (volgende.DalingPerSeconde <= 0) volgende.DalingPerSeconde = 0.10m;
                    if (volgende.ResterendeHoeveelheid <= 0) volgende.ResterendeHoeveelheid = volgende.Aanmelding?.Hoeveelheid ?? 0;

                    volgende.HuidigePrijs = volgende.MaximumPrijs;
                    volgende.LaatstePrijsUpdateUtc = DateTime.UtcNow; // ✅ reset timer
                    veiling.HuidigProductId = volgende.Id;
                }
                else
                {
                    // Sluit de veiling af als er geen volgende producten zijn
                    veiling.Status = VeilingStatus.Afgesloten;
                    veiling.AfgeslotenOpUtc ??= DateTime.UtcNow;
                    veiling.HuidigProductId = null;
                }
            }

            await _db.SaveChangesAsync();

            // Haalt koper-naam op voor DTO output
            var koperUser = await _db.Gebruikers.AsNoTracking().FirstOrDefaultAsync(g => g.Id == koperGebruikerId);

            // Geeft bod-info terug voor frontend
            return new BodDto
            {
                Id = bod.Id,
                Prijs = bod.Prijs,
                Tijdstip = bod.Tijdstip,
                KoperNaam = koperUser != null ? $"{koperUser.Voornaam} {koperUser.Achternaam}" : null
            };
        }



        // Geeft alle veilingdagen terug als strings voor dropdowns/filters
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
