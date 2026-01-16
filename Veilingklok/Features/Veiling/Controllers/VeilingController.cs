using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Time;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Route("api/veiling")]
    [Authorize(Roles = "Veilingmeester")]
    public class VeilingController : ControllerBase
    {
        private readonly IVeilingService _service;

        public VeilingController(IVeilingService service)
        {
            _service = service;
        }

        // Maak/plande een nieuwe veiling (status: gepland)
        // Endpoint: POST /api/veiling/start
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartVeilingDto dto)
        {
            // Basic validatie (voorkomt null/lege requests)
            if (dto == null)
                return BadRequest(new { message = "Request body ontbreekt." });

            // Default starttijd als die niet meegegeven is
            var startTijd = dto.StartTijd ?? new TimeSpan(9, 0, 0);

            // NL tijd "nu"
            var nowNl = NlTime.Now();

            // Geplande startmoment in NL tijd (Datum + StartTijd)
            var geplandeStartNl = dto.Veildatum.Date + startTijd;

            // Optioneel: blokkeer plannen in het verleden (of te dichtbij)
            if (geplandeStartNl < nowNl.AddMinutes(1))
            {
                return BadRequest(new
                {
                    message = $"Starttijd ligt te vroeg. Kies een starttijd na {nowNl.AddMinutes(1):yyyy-MM-dd HH:mm}."
                });
            }

            // Service uitvoeren
            var overzicht = await _service.StartVeilingAsync(
                dto.Veildatum,
                dto.LeverDatum,
                startTijd
            );

            return Ok(overzicht);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            return Ok(await _service.GetDetailsAsync(id));
        }

        [HttpPost("{id}/pause")]
        public async Task<IActionResult> Pause(int id)
        {
            await _service.PauseAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/resume")]
        public async Task<IActionResult> Resume(int id)
        {
            await _service.ResumeAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/stop")]
        public async Task<IActionResult> Stop(int id)
        {
            await _service.StopAsync(id);
            return NoContent();
        }

        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetVeilingDagen()
        {
            var dagen = await _service.GetVeilingDagenAsync();
            return Ok(dagen);
        }
    }
}
