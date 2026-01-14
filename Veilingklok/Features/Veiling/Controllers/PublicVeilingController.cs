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
    [AllowAnonymous]
    [Route("api/veiling-public")]
    [Produces("application/json")]
    // Public controller voor openbare veilinginformatie
    public class VeilingPublicController : ControllerBase
    {

        private readonly MyContext _db;

        // Injecteert database context
        public VeilingPublicController(MyContext db)
        {
            _db = db;
        }

        // Haalt alle beschikbare veildagen op
        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetPublicVeildagen()
        {
            var dagen = await _db.Aanmeldingen
                .Where(a => a.VeilingProductId == null)
                .Select(a => a.LeverDatum.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }

        [HttpGet("actief")]
        [Produces("application/json")]
        // Haalt de huidige actieve of gepauzeerde veiling op
        public async Task<IActionResult> GetActief()
        {
            var v = await _db.Veilingen
                .Include(x => x.Producten)
                    .ThenInclude(p => p.Aanmelding)
                        .ThenInclude(a => a.Aanvoerder)
                .FirstOrDefaultAsync(x =>
                    x.Status == VeilingStatus.Gestart ||
                    x.Status == VeilingStatus.Gepauzeerd);

            // Geeft lege response terug als er geen actieve veiling is
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

            // Bouwt basis veiling-overzicht
            var dto = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten
            };

            // Zet huidig product om naar DTO
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

            // Bouwt de wachtrij
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

            return Ok(dto);
        }



        [HttpGet("volgende")]
        // Haalt de eerstvolgende geplande veiling op
        public async Task<ActionResult<GeplandeVeilingListItemDto?>> GetVolgende()
        {
            var today = DateTime.Today;
            var nowTime = DateTime.Now.TimeOfDay;

            // Berekent tijdsgrens met kleine marge
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

            if (volgende == null) return Ok(null);

            // Telt hoeveel producten in deze veiling zitten
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
