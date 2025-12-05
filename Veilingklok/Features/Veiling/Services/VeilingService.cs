using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
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

        public async Task<VeilingOverzichtDto> StartVeilingAsync(DateTime veildatum)
        {
            var aanmeldingen = await _db.Aanmeldingen
                .Where(a => a.Veildatum.Date == veildatum.Date)
                .OrderBy(a => a.Soort)
                .ToListAsync();

            if (!aanmeldingen.Any())
                throw new ArgumentException("Geen aanmeldingen voor deze veildatum.");

            var veiling = new VeilingEntity
            {
                StartTijd = DateTime.UtcNow,
                IsGestart = true
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();

            int volgorde = 1;

            foreach (var a in aanmeldingen)
            {
                var product = new VeilingProduct
                {
                    VeilingId = veiling.Id,
                    AanmeldingId = a.Id,
                    StartPrijs = a.MinimumPrijs,
                    HuidigePrijs = a.MinimumPrijs,
                    Volgorde = volgorde++,
                };

                _db.VeilingProducten.Add(product);
            }

            await _db.SaveChangesAsync();

            var eerste = await _db.VeilingProducten
                .Where(vp => vp.VeilingId == veiling.Id)
                .OrderBy(vp => vp.Volgorde)
                .FirstAsync();

            eerste.IsActief = true;
            veiling.HuidigProductId = eerste.Id;

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
                IsGestart = v.IsGestart,
                IsPauze = v.IsPauze,
                IsAfgesloten = v.IsAfgesloten
            };

            // huidig product
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
                    FotoUrl = p.Aanmelding!.FotoUrl,
                    StartPrijs = p.StartPrijs,
                    Volgorde = p.Volgorde
                })
                .ToList();

            return dto;
        }

        public async Task PauseAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            v.IsPauze = true;
            await _db.SaveChangesAsync();
        }

        public async Task ResumeAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            v.IsPauze = false;
            await _db.SaveChangesAsync();
        }

        public async Task StopAsync(int veilingId)
        {
            var v = await _db.Veilingen.FindAsync(veilingId)
                ?? throw new ArgumentException("Veiling niet gevonden");

            v.IsAfgesloten = true;
            await _db.SaveChangesAsync();
        }

        public async Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperId)
        {
            var product = await _db.VeilingProducten
                .Include(p => p.Biedingen)
                .SingleAsync(p => p.Id == dto.VeilingProductId && p.VeilingId == veilingId);

            if (!product.IsActief)
                throw new ArgumentException("Product is niet actief.");

            product.IsVerkocht = true;
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

            // activeer volgend product
            var volgende = await _db.VeilingProducten
                .Where(p => p.VeilingId == veilingId && !p.IsVerkocht && !p.IsActief)
                .OrderBy(p => p.Volgorde)
                .FirstOrDefaultAsync();

            var veiling = await _db.Veilingen.FindAsync(veilingId);

            if (volgende != null)
            {
                volgende.IsActief = true;
                veiling!.HuidigProductId = volgende.Id;
            }
            else
            {
                veiling!.IsAfgesloten = true;
            }

            await _db.SaveChangesAsync();

            return new BodDto
            {
                Id = bod.Id,
                Prijs = bod.Prijs,
                Tijdstip = bod.Tijdstip
            };
        }
    }
}
