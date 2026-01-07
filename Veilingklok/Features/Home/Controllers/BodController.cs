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
            var koperId = await ResolveKoperIdAsync();
            if (koperId <= 0)
                return Unauthorized("Kon koper-id niet bepalen uit token.");

            var bod = await _service.PlaatsBodAsync(veilingId, dto, koperId);
            var overzicht = await _service.GetDetailsAsync(veilingId);

            // realtime events
            await _broadcast.StuurBod(veilingId, bod);

            if (overzicht.HuidigProduct != null)
                await _broadcast.StuurHuidigProduct(veilingId, overzicht.HuidigProduct);

            await _broadcast.StuurWachtrij(veilingId, overzicht.Wachtrij);

            await _broadcast.StuurAuditEvent(veilingId, new AuditEventDto
            {
                Gebeurtenis = $"Koop: VP#{dto.VeilingProductId} voor €{bod.Prijs:0.00} (aantal {(dto.Aantal <= 0 ? "alles" : dto.Aantal.ToString())}).",
                Tijdstip = DateTime.UtcNow
            });

            return Ok(bod);
        }

        private async Task<int> ResolveKoperIdAsync()
        {
            // Probeer verschillende claim keys, zodat dit werkt met jouw huidige JWT implementatie.
            var userIdStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("userId") ??
                User.FindFirstValue("id");

            if (!int.TryParse(userIdStr, out var gebruikerId))
                return 0;

            var koper = await _db.Kopers.SingleOrDefaultAsync(k => k.GebruikerId == gebruikerId);
            return koper?.Id ?? 0;
        }
    }
}
