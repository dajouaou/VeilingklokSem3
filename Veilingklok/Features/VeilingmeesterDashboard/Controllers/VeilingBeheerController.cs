using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Authorize(Roles = "Veilingmeester")]
    [Route("api/veilingmeester/veilingen")]
    public class VeilingBeheerController : ControllerBase
    {
        private readonly MyContext _db;
        private readonly IVeilingBroadcastService _broadcast;

        public VeilingBeheerController(MyContext db, IVeilingBroadcastService broadcast)
        {
            _db = db;
            _broadcast = broadcast;
        }

        [HttpGet("actief")]
        public async Task<ActionResult<VeilingOverzichtDto?>> GetActieve()
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .Include(v => v.HuidigProduct)
                    .ThenInclude(p => p.Aanmelding)
                .FirstOrDefaultAsync(v => v.Status == VeilingStatus.Gestart);

            if (v == null)
                return Ok(null);

            var overzicht = new VeilingOverzichtDto
            {
                Id = v.Id,
                IsGestart = v.Status == VeilingStatus.Gestart,
                IsPauze = v.Status == VeilingStatus.Gepauzeerd,
                IsAfgesloten = v.Status == VeilingStatus.Afgesloten,

                HuidigProduct = v.HuidigProduct == null ? null : new HuidigProductDto
                {
                    VeilingProductId = v.HuidigProduct.Id,
                    Soort = v.HuidigProduct.Aanmelding?.Soort ?? "Onbekend",
                    FotoUrl = v.HuidigProduct.Aanmelding?.FotoUrl ?? "",
                    Hoeveelheid = v.HuidigProduct.Aanmelding?.Hoeveelheid ?? 0,
                    StartPrijs = v.HuidigProduct.StartPrijs,
                    HuidigePrijs = v.HuidigProduct.HuidigePrijs,
                    IsActief = true,
                    IsVerkocht = v.HuidigProduct.IsVerkocht
                },

                Wachtrij = v.Producten
                    .Where(p => p.Id != v.HuidigProductId)
                    .OrderBy(p => p.Volgorde)
                    .Select(p => new WachtrijItemDto
                    {
                        VeilingProductId = p.Id,
                        Soort = p.Aanmelding?.Soort ?? "Onbekend",
                        Hoeveelheid = p.Aanmelding?.Hoeveelheid ?? 0,
                        Volgorde = p.Volgorde
                    })
                    .ToList()
            };

            return Ok(overzicht);
        }




        [HttpPost("{id}/start")]
        public async Task<ActionResult<VeilingOverzichtDto>> Start(int id)
        {
            var v = await _db.Veilingen
                .Include(v => v.Producten)
                    .ThenInclude(p => p.Aanmelding)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v == null) return NotFound();
            if (v.Status != VeilingStatus.Gepland)
                return BadRequest(new { message = "Deze veiling is niet (meer) gepland." });

            var first = v.Producten.OrderBy(p => p.Volgorde).FirstOrDefault();
            if (first == null)
                return BadRequest(new { message = "Geen producten in deze veiling." });

            v.Status = VeilingStatus.Gestart;
            v.HuidigProductId = first.Id;

            await _db.SaveChangesAsync();

            var huidigDto = new HuidigProductDto
            {
                VeilingProductId = first.Id,
                Soort = first.Aanmelding.Soort,
                FotoUrl = first.Aanmelding.FotoUrl,
                Hoeveelheid = first.Aanmelding.Hoeveelheid,
                HuidigePrijs = first.HuidigePrijs,
                StartPrijs = first.StartPrijs
            };

            var wachtrijDto = v.Producten
                .Where(p => p.Id != first.Id)
                .OrderBy(p => p.Volgorde)
                .Select(p => new WachtrijItemDto
                {
                    VeilingProductId = p.Id,
                    Soort = p.Aanmelding.Soort,
                    Hoeveelheid = p.Aanmelding.Hoeveelheid,
                    Volgorde = p.Volgorde
                })
                .ToList();

            await _broadcast.StuurHuidigProduct(v.Id, huidigDto);
            await _broadcast.StuurWachtrij(v.Id, wachtrijDto);
            await _broadcast.StuurAuditEvent(v.Id, new AuditEventDto
            {
                Gebeurtenis = "Veiling gestart",
                Tijdstip = DateTime.UtcNow,
                UitgevoerdDoor = User.Identity?.Name
            });

            return await GetActieve();
        }
    }
}
