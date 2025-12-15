using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Home.Controllers
{
    [ApiController]
    [Authorize(Roles = "Koper")]
    [Route("api/bod")]
    public class BodController : ControllerBase
    {
        private readonly IVeilingService _service;
        private readonly IVeilingBroadcastService _broadcast;

        public BodController(
            IVeilingService service,
            IVeilingBroadcastService broadcast)
        {
            _service = service;
            _broadcast = broadcast;
        }

        [HttpPost("{veilingId}")]
        public async Task<ActionResult<BodDto>> PlaatsBod(
            int veilingId,
            [FromBody] BodPlaatsenDto dto)
        {
            int koperId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            // 1️⃣ Bod plaatsen (businesslogica)
            var bod = await _service.PlaatsBodAsync(veilingId, dto, koperId);

            // 2️⃣ Nieuwe veilingstatus ophalen
            var overzicht = await _service.GetDetailsAsync(veilingId);

            // 3️⃣ Realtime updates sturen
            await _broadcast.StuurBod(veilingId, bod);

            if (overzicht.HuidigProduct != null)
            {
                await _broadcast.StuurHuidigProduct(
                    veilingId,
                    overzicht.HuidigProduct
                );
            }

            await _broadcast.StuurWachtrij(
                veilingId,
                overzicht.Wachtrij
            );

            await _broadcast.StuurAuditEvent(veilingId, new AuditEventDto
            {
                Gebeurtenis = $"Bod geplaatst: € {bod.Prijs:F2}",
                Tijdstip = DateTime.UtcNow
            });

            // 4️⃣ Bod teruggeven aan caller (optioneel)
            return Ok(bod);
        }
    }
}
