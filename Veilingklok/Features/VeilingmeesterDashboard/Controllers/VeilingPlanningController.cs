using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Authorize(Roles = "Veilingmeester")]
    [Route("api/veilingmeester/planning")]
    public class VeilingPlanningController : ControllerBase
    {
        private readonly MyContext _db;

        public VeilingPlanningController(MyContext db)
        {
            _db = db;
        }

        [HttpGet("veildagen")]
        public async Task<IActionResult> GetVeildagen()
        {
            var today = DateTime.Today;

            var dagen = await _db.Aanmeldingen
                .Include(a => a.VeilingProduct)
                .Where(a => a.LeverDatum.Date >= today && a.VeilingProduct == null)
                .Select(a => a.LeverDatum.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(dagen.Select(d => d.ToString("yyyy-MM-dd")));
        }



        // 2️⃣ Aanmeldingen per leverdatum
        [HttpGet("aanmeldingen")]
        public async Task<IActionResult> GetAanmeldingen([FromQuery] string leverdatum)
        {
            if (!DateTime.TryParse(leverdatum, out var parsedDatum))
                return BadRequest("Leverdatum ongeldig (yyyy-MM-dd)");

            var items = await _db.Aanmeldingen
                .Include(a => a.Aanvoerder)
                .Include(a => a.VeilingProduct)
                .Where(a =>
                    a.LeverDatum.Date == parsedDatum.Date &&
                    a.VeilingProduct == null
                )
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

            return Ok(items);
        }

        [HttpPost("plan")]
        public async Task<IActionResult> PlanVeiling([FromBody] PlanVeilingRequestDto dto)
        {
            if (!DateTime.TryParse(dto.Leverdatum, out var leverdatum))
                return BadRequest("Leverdatum ongeldig");

            if (!DateTime.TryParse(dto.Veildatum, out var veildatum))
                return BadRequest("Veildatum ongeldig");

            if (!TimeSpan.TryParse(dto.StartTijd, out var startTijd))
                return BadRequest("Starttijd ongeldig");

            // ✅ voorkom “direct afgesloten” door cleanup service
            var geplandeStart = veildatum.Date + startTijd;
            if (geplandeStart <= DateTime.Now.AddMinutes(1))
                return BadRequest($"Je kunt geen veiling plannen in het verleden. Kies een tijd na {DateTime.Now.AddMinutes(1):yyyy-MM-dd HH:mm}.");

            // ✅ zoek op datum + starttijd (anders overschrijf je onbedoeld of haal je de verkeerde op)
            var veiling = await _db.Veilingen
                .Include(v => v.Producten)
                .FirstOrDefaultAsync(v =>
                    v.Status == VeilingStatus.Gepland &&
                    v.Datum.Date == veildatum.Date &&
                    v.StartTijd == startTijd
                );

            if (veiling == null)
            {
                veiling = new VeilingEntity
                {
                    Datum = veildatum.Date,
                    StartTijd = startTijd,
                    Status = VeilingStatus.Gepland,
                    Producten = new List<VeilingProduct>()
                };

                _db.Veilingen.Add(veiling);
                await _db.SaveChangesAsync();
            }

            var producten = veiling.Producten ?? new List<VeilingProduct>();

            var bestaandeAanmeldingen = producten
                .Select(p => p.AanmeldingId)
                .ToHashSet();

            var dubbeleAanmeldingen = dto.AanmeldingIds
                .Where(id => bestaandeAanmeldingen.Contains(id))
                .ToList();

            if (dubbeleAanmeldingen.Any())
                return BadRequest("Geselecteerde producten zijn al aangemeld voor de veiling.");

            int volgorde = producten.Any()
                ? producten.Max(p => p.Volgorde) + 1
                : 1;

            foreach (var aanmeldingId in dto.AanmeldingIds)
            {
                var a = await _db.Aanmeldingen.FindAsync(aanmeldingId);
                if (a == null || a.VeilingProduct != null) continue;

                var maximumPrijs = a.MinimumPrijs + 1.00m;
                var daling = 0.05m;

                var vp = new VeilingProduct
                {
                    VeilingId = veiling.Id,
                    AanmeldingId = a.Id,

                    MaximumPrijs = maximumPrijs,
                    MinimumPrijs = a.MinimumPrijs,
                    HuidigePrijs = maximumPrijs,

                    DalingPerSeconde = daling,
                    ResterendeHoeveelheid = a.Hoeveelheid,
                    Volgorde = volgorde++,
                };

                a.VeilingProduct = vp;
                _db.VeilingProducten.Add(vp);
            }

            await _db.SaveChangesAsync();

            return Ok(new { veilingId = veiling.Id });
        }


        [HttpGet("gepland")]
        public async Task<IActionResult> GetGeplande()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            var grace = TimeSpan.FromMinutes(5);
            var cutoff = nowTime - grace;
            if (cutoff < TimeSpan.Zero) cutoff = TimeSpan.Zero;

            var veilingen = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland &&
                    (v.Datum > today || (v.Datum == today && v.StartTijd >= cutoff)))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .ToListAsync();

            var productCounts = await _db.VeilingProducten
                .GroupBy(p => p.VeilingId)
                .Select(g => new { VeilingId = g.Key, Aantal = g.Count() })
                .ToListAsync();

            var result = veilingen.Select(v =>
            {
                var aantal = productCounts.FirstOrDefault(x => x.VeilingId == v.Id)?.Aantal ?? 0;

                return new GeplandeVeilingListItemDto
                {
                    Id = v.Id,
                    Veildatum = v.Datum.ToString("yyyy-MM-dd"),
                    StartTijd = v.StartTijd.ToString(@"hh\:mm"),
                    AantalProducten = aantal
                };
            }).ToList();

            return Ok(result);
        }



        [HttpGet("volgende")]
        public async Task<IActionResult> GetVolgende()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            var grace = TimeSpan.FromMinutes(5);
            var cutoff = nowTime - grace;
            if (cutoff < TimeSpan.Zero) cutoff = TimeSpan.Zero;

            var volgende = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland &&
                    (v.Datum > today || (v.Datum == today && v.StartTijd >= cutoff)))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .FirstOrDefaultAsync();

            if (volgende == null)
                return Ok(null);

            var aantal = await _db.VeilingProducten.CountAsync(p => p.VeilingId == volgende.Id);

            return Ok(new GeplandeVeilingListItemDto
            {
                Id = volgende.Id,
                Veildatum = volgende.Datum.ToString("yyyy-MM-dd"),
                StartTijd = volgende.StartTijd.ToString(@"hh\:mm"),
                AantalProducten = aantal
            });
        }



    }
}
