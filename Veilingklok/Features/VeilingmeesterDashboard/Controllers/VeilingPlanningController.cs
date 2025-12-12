using Humanizer;
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

        // 1️⃣ VEILDAGEN OPHALEN
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

        [HttpGet("aanmeldingen")]
        public async Task<IActionResult> GetAanmeldingen([FromQuery] DateTime leverdatum)
        {
            var items = await _db.Aanmeldingen
                .Include(a => a.Aanvoerder)
                .Where(a =>
                    a.LeverDatum.Date == leverdatum.Date &&
                    a.VeilingProductId == null
                )
                .Select(a => new VeilingPlanningAanmeldingDto
                {
                    Id = a.Id,
                    Soort = a.Soort,
                    Hoeveelheid = a.Hoeveelheid,
                    MinimumPrijs = a.MinimumPrijs,
                    AanvoerderNaam = a.Aanvoerder!.Naam,
                    LeverDatum = a.LeverDatum,
                })
                .ToListAsync();

            return Ok(items);
        }



        [HttpPost("plan")]
        public async Task<ActionResult> PlanVeiling([FromBody] PlanVeilingRequestDto dto)
        {
            if (dto.AanmeldingIds == null || dto.AanmeldingIds.Count == 0)
                return BadRequest(new { message = "Geen producten geselecteerd." });

            if (!TimeSpan.TryParse(dto.StartTijd, out var tijd))
                return BadRequest(new { message = "Starttijd ongeldig." });

            // ⭐ NIEUW: check of deze veildatum al gepland is
            var bestaatAl = await _db.Veilingen
                .AnyAsync(v => v.Datum == dto.Veildatum.Date && v.Status == VeilingStatus.Gepland);

            if (bestaatAl)
                return BadRequest(new { message = "Er bestaat al een geplande veiling voor deze datum." });

            var veiling = new VeilingEntity
            {
                Datum = dto.Veildatum.Date,
                StartTijd = tijd,
                Status = VeilingStatus.Gepland
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();

            int volgorde = 1;

            foreach (var id in dto.AanmeldingIds)
            {
                var a = await _db.Aanmeldingen.FindAsync(id);
                if (a == null) continue;

                _db.VeilingProducten.Add(new VeilingProduct
                {
                    VeilingId = veiling.Id,
                    AanmeldingId = a.Id,
                    Volgorde = volgorde++,
                    StartPrijs = a.MinimumPrijs,
                    HuidigePrijs = a.MinimumPrijs,
                    IsVerkocht = false
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Veiling gepland.",
                veilingId = veiling.Id
            });
        }


        [HttpGet("gepland")]
        public async Task<IActionResult> GetGeplande()
        {
            var veilingen = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland)
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .ToListAsync();

            if (!veilingen.Any())
                return Ok(new List<GeplandeVeilingListItemDto>());

            var productCounts = await _db.VeilingProducten
                .GroupBy(p => p.VeilingId)
                .Select(g => new
                {
                    VeilingId = g.Key,
                    Aantal = g.Count()
                })
                .ToListAsync();

            var result = veilingen.Select(v =>
            {
                var count = productCounts
                    .FirstOrDefault(x => x.VeilingId == v.Id)?.Aantal ?? 0;

                return new GeplandeVeilingListItemDto
                {
                    Id = v.Id,
                    Veildatum = v.Datum.ToString("yyyy-MM-dd"),
                    StartTijd = v.StartTijd.ToString(@"hh\:mm"),
                    AantalProducten = count
                };
            }).ToList();

            return Ok(result);
        }

        [HttpGet("leverdata")]
        public async Task<IActionResult> GetLeverdata()
        {
            var data = await _db.Aanmeldingen
                .Select(a => a.LeverDatum.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(data.Select(d => d.ToString("yyyy-MM-dd")));
        }






    }
}
