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

        // 1️⃣ Leverdatums waar aanmeldingen zijn
        [HttpGet("veildagen")]
        public async Task<IActionResult> GetVeildagen()
        {
            var dagen = await _db.Aanmeldingen
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
                .Where(a =>
                    a.LeverDatum.Date == parsedDatum.Date &&
                    a.VeilingProductId == null
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

            var veiling = await _db.Veilingen
                .Include(v => v.Producten)
                .FirstOrDefaultAsync(v =>
                    v.Status == VeilingStatus.Gepland &&
                    v.Datum.Date == veildatum.Date
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
            {
                return BadRequest("Geselecteerde producten zijn al aangemeld voor de veiling.");
            }

            int volgorde = producten.Any()
                ? producten.Max(p => p.Volgorde) + 1
                : 1;


            foreach (var id in dto.AanmeldingIds)
            {
                if (bestaandeAanmeldingen.Contains(id))
                    continue;

                var a = await _db.Aanmeldingen.FindAsync(id);
                if (a == null || a.VeilingProduct != null)
                    continue;

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

            await _db.SaveChangesAsync();

            return Ok(new { veilingId = veiling.Id });
        }


        // 4️⃣ Geplande veilingen
        [HttpGet("gepland")]
        public async Task<IActionResult> GetGeplande()
        {
            var veilingen = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland)
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .ToListAsync();

            var productCounts = await _db.VeilingProducten
                .GroupBy(p => p.VeilingId)
                .Select(g => new { VeilingId = g.Key, Aantal = g.Count() })
                .ToListAsync();

            var result = veilingen.Select(v =>
            {
                var aantal = productCounts
                    .FirstOrDefault(x => x.VeilingId == v.Id)?.Aantal ?? 0;

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
    }
}
