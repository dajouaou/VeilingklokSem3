using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Controllers
{
    // Deze controller bevat publieke endpoints
    // Iedereen mag deze endpoints aanroepen (geen login vereist)
    [ApiController]
    [AllowAnonymous]
    [Route("api/veiling-public")]
    [Produces("application/json")]
    public class VeilingPublicController : ControllerBase
    {
        // Database context voor het ophalen van veilingdata
        private readonly MyContext _db;

        // Constructor: DbContext wordt via dependency injection meegegeven
        public VeilingPublicController(MyContext db)
        {
            _db = db;
        }

        // Geeft alle beschikbare veildagen terug
        // Dit zijn dagen waarvoor nog aanmeldingen bestaan
        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetPublicVeildagen()
        {
            // Haal alle leverdata op van aanmeldingen die nog niet ingepland zijn
            var dagen = await _db.Aanmeldingen
                .Where(a => a.VeilingProductId == null) // alleen nog niet ingeplande aanmeldingen
                .Select(a => a.LeverDatum.Date)         // alleen datum, geen tijd
                .Distinct()                             // dubbele datums verwijderen
                .OrderBy(d => d)                        // sorteren op datum
                .ToListAsync();

            // Datums omzetten naar string-formaat voor de frontend
            return dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }

        // Haalt de huidige actieve of gepauzeerde veiling op
        [HttpGet("actief")]
        [Produces("application/json")]
        public async Task<IActionResult> GetActief()
        {
            // Zoek een veiling die gestart of gepauzeerd is
            var v = await _db.Veilingen
                .Include(x => x.Producten)
                    .ThenInclude(p => p.Aanmelding)
                        .ThenInclude(a => a.Aanvoerder)
                .FirstOrDefaultAsync(x =>
                    x.Status == VeilingStatus.Gestart ||
                    x.Status == VeilingStatus.Gepauzeerd);

            // Als er geen actieve veiling is, geef een leeg overzicht terug
            if (v == null)
            {
                return Ok(new VeilingOverzichtDto
                {
                    Id = 0,
                    IsGestart = false,
                    IsPauze = false,
                    IsAfgesloten = false,
                    HuidigProduct = null,
                    Wachtrij = new List<WachtrijItemDto>()
                });
            }

            // Basisinformatie van de veiling
            var dto = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten
            };

            // Huidig actief product bepalen
            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);

            // Als er een huidig product is, vul deze in het DTO
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

            // Wachtrij vullen met producten die nog moeten komen
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
                    AanvoerderNaam = p.Aanmelding!.Aanvoerder != null
                        ? p.Aanmelding!.Aanvoerder!.Naam
                        : ""
                })
                .ToList();

            // Volledig veilingoverzicht teruggeven
            return Ok(dto);
        }

        // Geeft de eerstvolgende geplande veiling terug
        [HttpGet("volgende")]
        public async Task<ActionResult<GeplandeVeilingListItemDto?>> GetVolgende()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            // Kleine marge van 5 minuten om randgevallen te voorkomen
            var grace = TimeSpan.FromMinutes(5);
            var cutoff = nowTime - grace;
            if (cutoff < TimeSpan.Zero) cutoff = TimeSpan.Zero;

            // Zoek de eerstvolgende geplande veiling
            var volgende = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland &&
                            (v.Datum > today ||
                             (v.Datum == today && v.StartTijd >= cutoff)))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .FirstOrDefaultAsync();

            // Geen geplande veiling gevonden
            if (volgende == null)
                return Ok(null);

            // Aantal producten in de geplande veiling bepalen
            var aantal = await _db.VeilingProducten
                .CountAsync(p => p.VeilingId == volgende.Id);

            // DTO met geplande veilinginformatie teruggeven
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
