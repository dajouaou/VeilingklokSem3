using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Authorize(Roles = "Koper")]
    [Route("api/bod")]
    public class BodController : ControllerBase
    {
        private readonly IVeilingService _service;
        private readonly IVeilingBroadcastService _broadcast;
        private readonly MyContext _db;

        public BodController(IVeilingService service, IVeilingBroadcastService broadcast, MyContext db)
        {
            _service = service;
            _broadcast = broadcast;
            _db = db;
        }

        [HttpPost("{veilingId:int}")]
        public async Task<IActionResult> Plaats(int veilingId, [FromBody] BodPlaatsenDto dto)
        {
            var koperGebruikerId = await ResolveKoperGebruikerIdAsync();
            if (koperGebruikerId <= 0)
                return Unauthorized("Kon koper-id niet bepalen uit token.");

            var bod = await _service.PlaatsBodAsync(veilingId, dto, koperGebruikerId);
            var overzicht = await _service.GetDetailsAsync(veilingId);

            await _broadcast.StuurBod(veilingId, bod);

            if (overzicht.HuidigProduct != null)
                await _broadcast.StuurHuidigProduct(veilingId, overzicht.HuidigProduct);

            await _broadcast.StuurWachtrij(veilingId, overzicht.Wachtrij);

            await _broadcast.StuurAuditEvent(veilingId, new AuditEventDto
            {
                Gebeurtenis = $"Koop: VP#{dto.VeilingProductId} voor {bod.Prijs:0.00} EUR (aantal {(dto.Aantal <= 0 ? "alles" : dto.Aantal.ToString())}).",
                Tijdstip = DateTime.UtcNow
            });

            return Ok(bod);
        }


        private async Task<int> ResolveKoperGebruikerIdAsync()
        {
            var userIdStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("userId") ??
                User.FindFirstValue("id");

            if (!int.TryParse(userIdStr, out var gebruikerId))
                return 0;

            // check dat deze gebruiker ook echt een koper-profiel heeft
            var exists = await _db.Kopers.AnyAsync(k => k.GebruikerId == gebruikerId);
            return exists ? gebruikerId : 0;
        }

    }
}
