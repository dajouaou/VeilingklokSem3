using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Authorize(Roles = "Veilingmeester")]
    [Route("api/veilingmeester/veilingen")]
    public class VeilingBeheerController : ControllerBase
    {
        private readonly IVeilingService _service;
        private readonly IVeilingBroadcastService _broadcast;

        public VeilingBeheerController(
            IVeilingService service,
            IVeilingBroadcastService broadcast)
        {
            _service = service;
            _broadcast = broadcast;
        }

        [HttpGet("actief")]
        public async Task<ActionResult<VeilingOverzichtDto?>> GetActieve()
        {
            return Ok(await _service.GetActieveVeilingAsync());
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult<VeilingOverzichtDto>> Start(int id)
        {
            var overzicht = await _service.StartGeplandeVeilingAsync(id);

            if (overzicht.HuidigProduct != null)
            {
                await _broadcast.StuurHuidigProduct(id, overzicht.HuidigProduct);
                await _broadcast.StuurWachtrij(id, overzicht.Wachtrij);
                await _broadcast.StuurAuditEvent(id, new AuditEventDto
                {
                    Gebeurtenis = "Veiling gestart",
                    Tijdstip = DateTime.UtcNow
                });
            }

            return Ok(overzicht);
        }

        [HttpPost("{id}/pause")]
        public async Task<IActionResult> Pause(int id)
        {
            await _service.PauseAsync(id);

            await _broadcast.StuurAuditEvent(id, new AuditEventDto
            {
                Gebeurtenis = "Veiling gepauzeerd",
                Tijdstip = DateTime.UtcNow
            });

            return NoContent();
        }

        [HttpPost("{id}/resume")]
        public async Task<IActionResult> Resume(int id)
        {
            await _service.ResumeAsync(id);

            await _broadcast.StuurAuditEvent(id, new AuditEventDto
            {
                Gebeurtenis = "Veiling hervat",
                Tijdstip = DateTime.UtcNow
            });

            return NoContent();
        }

        [HttpPost("{id}/stop")]
        public async Task<IActionResult> Stop(int id)
        {
            await _service.StopAsync(id);

            await _broadcast.StuurAuditEvent(id, new AuditEventDto
            {
                Gebeurtenis = "Veiling gestopt",
                Tijdstip = DateTime.UtcNow
            });

            return NoContent();
        }
    }
}
