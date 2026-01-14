using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [AllowAnonymous] // alles in deze controller is publiek toegankelijk
    [Route("api/veiling-public")]
    [Produces("application/json")]
    public class VeilingPublicController : ControllerBase
    {
        private readonly MyContext _db;

        public VeilingPublicController(MyContext db)
        {
            _db = db; // DbContext via dependency injection
        }

        // Geeft unieke veildagen terug waarvoor nog geen veilingproduct is aangemaakt
        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetPublicVeildagen()
        {
            var dagen = await _db.Aanmeldingen
                .Where(a => a.VeilingProductId == null) // alleen nog niet ingeplande aanmeldingen
                .Select(a => a.LeverDatum.Date)         // alleen de datum, zonder tijd
                .Distinct()                             // dubbele dagen voorkomen
                .OrderBy(d => d)                        // chronologisch sorteren
                .ToListAsync();

            return dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }

        // Haalt de actieve (gestarte of gepauzeerde) veiling op
        [HttpGet("actief")]
        [Produces("application/json")]
        public async Task<IActionResult> GetActief()
        {
            var v = await _db.Veilingen
                .Include(x => x.Producten)
                    .ThenInclude(p => p.Aanmelding)
                        .ThenInclude(a => a.Aanvoerder)
                .FirstOrDefaultAsync(x =>
                    x.Status == VeilingStatus.Gestart ||
                    x.Status == VeilingStatus.Gepauzeerd);

            // Geen actieve veiling → lege maar geldige response
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

            // Basisinformatie over de veilingstatus
            var dto = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten
            };

            // Huidig actief product bepalen
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

            // Wachtrij: producten die nog niet actief/verkocht/doorgedraaid zijn
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

            return Ok(dto);
        }

        // Geeft de eerstvolgende geplande veiling terug
        [HttpGet("volgende")]
        public async Task<ActionResult<GeplandeVeilingListItemDto?>> GetVolgende()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            // 5 minuten speling zodat net begonnen veilingen niet meteen verdwijnen
            var grace = TimeSpan.FromMinutes(5);
            var cutoff = nowTime - grace;
            if (cutoff < TimeSpan.Zero) cutoff = TimeSpan.Zero;

            var volgende = await _db.Veilingen
                .Where(v => v.Status == VeilingStatus.Gepland &&
                            (v.Datum > today ||
                             (v.Datum == today && v.StartTijd >= cutoff)))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.StartTijd)
                .FirstOrDefaultAsync();

            if (volgende == null) return Ok(null);

            // Aantal producten apart tellen
            var aantal = await _db.VeilingProducten
                .CountAsync(p => p.VeilingId == volgende.Id);

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
