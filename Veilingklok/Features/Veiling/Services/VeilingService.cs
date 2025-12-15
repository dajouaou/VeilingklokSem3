using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;

namespace Veilingklok.Features.Veiling.Services
{
    public class VeilingService : IVeilingService
    {
        private readonly MyContext _db;

        public VeilingService(MyContext db)
        {
            _db = db;
        }

        public async Task<VeilingOverzichtDto?> GetActieveVeilingAsync()
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .FirstOrDefaultAsync(v => v.Status == VeilingStatus.Gestart);

            if (v == null) return null;

            return await GetDetailsAsync(v.Id);
        }

        public async Task<VeilingOverzichtDto> StartGeplandeVeilingAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .SingleOrDefaultAsync(v => v.Id == veilingId);

            if (v == null)
                throw new ArgumentException("Veiling bestaat niet.");

            if (v.Status != VeilingStatus.Gepland)
                throw new ArgumentException("Veiling is niet gepland.");

            var geplandeStart = v.Datum.Date + v.StartTijd;
            if (DateTime.Now < geplandeStart)
                throw new ArgumentException(
                    $"Deze veiling kan pas gestart worden op {geplandeStart:yyyy-MM-dd HH:mm}"
                );

            var first = v.Producten
                .OrderBy(p => p.Volgorde)
                .FirstOrDefault()
                ?? throw new ArgumentException("Geen producten in veiling.");

            v.Status = VeilingStatus.Gestart;
            v.HuidigProductId = first.Id;
            first.IsActief = true;

            await _db.SaveChangesAsync();

            return await GetDetailsAsync(v.Id);
        }


        public async Task<VeilingOverzichtDto> StartVeilingAsync(
     DateTime veilingDatum,
     DateTime leverDatum,
     TimeSpan? startTijd = null)
        {
            var aanmeldingen = await _db.Aanmeldingen
                .Where(a =>
                    a.LeverDatum.Date == leverDatum.Date &&
                    a.VeilingProductId == null // nog niet ingepland
                )
                .OrderBy(a => a.Soort)
                .ToListAsync();

            if (!aanmeldingen.Any())
                throw new ArgumentException("Geen aanmeldingen voor deze leverdatum.");

            var tijd = startTijd ?? new TimeSpan(9, 0, 0);

            var veiling = new VeilingEntity
            {
                Datum = veilingDatum.Date,  
                StartTijd = tijd,
                Status = VeilingStatus.Gepland
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();

            int volgorde = 1;

            foreach (var a in aanmeldingen)
            {
                var vp = new VeilingProduct
                {
                    VeilingId = veiling.Id,
                    AanmeldingId = a.Id,
                    StartPrijs = a.MinimumPrijs,
                    HuidigePrijs = a.MinimumPrijs,
                    Volgorde = volgorde++
                };

                _db.VeilingProducten.Add(vp);
                a.VeilingProduct = vp; // koppeling
            }

            await _db.SaveChangesAsync();

            return await GetDetailsAsync(veiling.Id);
        }

        public async Task<VeilingOverzichtDto> GetDetailsAsync(int veilingId)
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .SingleAsync(v => v.Id == veilingId);

            var dto = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten
            };

            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);

            if (hp != null)
            {
                dto.HuidigProduct = new HuidigProductDto
                {
                    VeilingProductId = hp.Id,
                    Soort = hp.Aanmelding!.Soort,
                    FotoUrl = hp.Aanmelding.FotoUrl,
                    StartPrijs = hp.StartPrijs,
                    HuidigePrijs = hp.HuidigePrijs,
                    Hoeveelheid = hp.Aanmelding.Hoeveelheid,
                    IsActief = hp.IsActief,
                    IsVerkocht = hp.IsVerkocht
                };
            }

            dto.Wachtrij = v.Producten
                .Where(p => !p.IsActief && !p.IsVerkocht)
                .OrderBy(p => p.Volgorde)
                .Select(p => new WachtrijItemDto
                {
                    VeilingProductId = p.Id,
                    Soort = p.Aanmelding!.Soort,
                    FotoUrl = p.Aanmelding.FotoUrl,
                    StartPrijs = p.StartPrijs,
                    Hoeveelheid = p.Aanmelding.Hoeveelheid,
                    Volgorde = p.Volgorde
                })
                .ToList();

            return dto;
        }
        public async Task PauseAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            v.Status = VeilingStatus.Gepauzeerd;
            await _db.SaveChangesAsync();
        }

        public async Task ResumeAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            v.Status = VeilingStatus.Gestart;
            await _db.SaveChangesAsync();
        }

        public async Task StopAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            v.Status = VeilingStatus.Afgesloten;
            await _db.SaveChangesAsync();
        }

  
        public async Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperId)
        {
            var product = await _db.VeilingProducten
                .Include(p => p.Aanmelding)
                .SingleAsync(p => p.Id == dto.VeilingProductId && p.VeilingId == veilingId);

            if (!product.IsActief)
                throw new ArgumentException("Product is niet actief.");

            product.IsVerkocht = true;
            product.IsActief = false;
            product.KoperId = koperId;
            product.HuidigePrijs = dto.Prijs;

            var bod = new Bod
            {
                VeilingId = veilingId,
                VeilingProductId = product.Id,
                KoperId = koperId,
                Prijs = dto.Prijs
            };

            _db.Biedingen.Add(bod);

            var volgende = await _db.VeilingProducten
                .Where(pv => pv.VeilingId == veilingId && !pv.IsVerkocht && !pv.IsActief)
                .OrderBy(pv => pv.Volgorde)
                .FirstOrDefaultAsync();

            var veiling = await _db.Veilingen.FindAsync(veilingId);

            if (volgende != null)
            {
                volgende.IsActief = true;
                veiling!.HuidigProductId = volgende.Id;
            }
            else
            {
                veiling!.Status = VeilingStatus.Afgesloten;
            }

            await _db.SaveChangesAsync();

            return new BodDto
            {
                Id = bod.Id,
                Prijs = bod.Prijs,
                Tijdstip = bod.Tijdstip
            };
        }

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
