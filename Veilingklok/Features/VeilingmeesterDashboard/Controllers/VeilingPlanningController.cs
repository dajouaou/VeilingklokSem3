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
    // Controller voor plannen en ophalen van geplande veilingen
    public class VeilingPlanningController : ControllerBase
    {
        private readonly MyContext _db;

        // Injecteert database context
        public VeilingPlanningController(MyContext db)
        {
            _db = db;
        }

        [HttpGet("veildagen")]
        // Haalt toekomstige leverdagen op waar nog aanmeldingen zonder veilingproduct zijn
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



        // Endpoint om aanmeldingen op te halen voor een specifieke leverdatum
        [HttpGet("aanmeldingen")]
        public async Task<IActionResult> GetAanmeldingen([FromQuery] string leverdatum)
        {
            // Valideert leverdatum input
            if (!DateTime.TryParse(leverdatum, out var parsedDatum))
                return BadRequest("Leverdatum ongeldig (yyyy-MM-dd)");

            // Haalt aanmeldingen op die nog niet ingepland zijn
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
        // Plant een veiling en koppelt geselecteerde aanmeldingen als veilingproducten
        public async Task<IActionResult> PlanVeiling([FromBody] PlanVeilingRequestDto dto)
        {
            // Valideert datum/tijd input
            if (!DateTime.TryParse(dto.Leverdatum, out var leverdatum))
                return BadRequest("Leverdatum ongeldig");

            if (!DateTime.TryParse(dto.Veildatum, out var veildatum))
                return BadRequest("Veildatum ongeldig");

            if (!TimeSpan.TryParse(dto.StartTijd, out var startTijd))
                return BadRequest("Starttijd ongeldig");

            // Blokkeert plannen in het verleden of te dicht op nu
            var geplandeStart = veildatum.Date + startTijd;
            if (geplandeStart <= DateTime.Now.AddMinutes(1))
                return BadRequest($"Je kunt geen veiling plannen in het verleden. Kies een tijd na {DateTime.Now.AddMinutes(1):yyyy-MM-dd HH:mm}.");

            // Zoekt bestaande geplande veiling op dezelfde datum en starttijd
            var veiling = await _db.Veilingen
                .Include(v => v.Producten)
                .FirstOrDefaultAsync(v =>
                    v.Status == VeilingStatus.Gepland &&
                    v.Datum.Date == veildatum.Date &&
                    v.StartTijd == startTijd
                );

            // Maakt een nieuwe geplande veiling als die nog niet bestaat
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

            // Gebruikt bestaande productenlijst als die er al is
            var producten = veiling.Producten ?? new List<VeilingProduct>();

            // Bouwt set van aanmeldingen die al in deze veiling zitten
            var bestaandeAanmeldingen = producten
                .Select(p => p.AanmeldingId)
                .ToHashSet();

            // Checkt op dubbele selectie
            var dubbeleAanmeldingen = dto.AanmeldingIds
                .Where(id => bestaandeAanmeldingen.Contains(id))
                .ToList();

            if (dubbeleAanmeldingen.Any())
                return BadRequest("Geselecteerde producten zijn al aangemeld voor de veiling.");

            // Bepaalt startvolgorde voor nieuwe items
            int volgorde = producten.Any()
                ? producten.Max(p => p.Volgorde) + 1
                : 1;

            // Koppelt elke aanmelding als veilingproduct aan deze veiling
            foreach (var aanmeldingId in dto.AanmeldingIds)
            {
                var a = await _db.Aanmeldingen.FindAsync(aanmeldingId);
                if (a == null || a.VeilingProduct != null) continue;

                var maximumPrijs = a.MinimumPrijs + 5.00m;    // start 5 euro boven min
                var daling = 0.10m;                         

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

            // Geeft het veilingId terug zodat de frontend ermee verder kan
            return Ok(new { veilingId = veiling.Id });
        }


        [HttpGet("gepland")]
        // Haalt alle toekomstige geplande veilingen op (met een kleine grace marge)
        public async Task<IActionResult> GetGeplande()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            var grace = TimeSpan.FromMinutes(5);
            var cutoff = nowTime - grace;
            if (cutoff < TimeSpan.Zero) cutoff = TimeSpan.Zero;

            // Haalt geplande veilingen op die nog relevant zijn
            var veilingen = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland &&
                    (v.Datum > today || (v.Datum == today && v.StartTijd >= cutoff)))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .ToListAsync();

            // Haalt per veiling het aantal producten op
            var productCounts = await _db.VeilingProducten
                .GroupBy(p => p.VeilingId)
                .Select(g => new { VeilingId = g.Key, Aantal = g.Count() })
                .ToListAsync();

            // Zet veilingen om naar lijst-items voor de UI
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
        // Haalt de eerstvolgende geplande veiling op
        public async Task<IActionResult> GetVolgende()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            var grace = TimeSpan.FromMinutes(5);
            var cutoff = nowTime - grace;
            if (cutoff < TimeSpan.Zero) cutoff = TimeSpan.Zero;

            // Zoekt de eerstvolgende geplande veiling
            var volgende = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland &&
                    (v.Datum > today || (v.Datum == today && v.StartTijd >= cutoff)))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .FirstOrDefaultAsync();

            if (volgende == null)
                return Ok(null);

            // Telt hoeveel producten in de veiling zitten
            var aantal = await _db.VeilingProducten.CountAsync(p => p.VeilingId == volgende.Id);

            // Geeft geplande veiling-info terug
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
