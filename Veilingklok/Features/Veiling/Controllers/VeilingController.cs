using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Time;

namespace Veilingklok.Features.Veiling.Controllers
{
    // Controller voor acties die alleen door de veilingmeester uitgevoerd mogen worden
    [ApiController]
    [Route("api/veiling")]
    [Authorize(Roles = "Veilingmeester")]
    public class VeilingController : ControllerBase
    {
        // Service waarin de businesslogica van de veiling zit
        private readonly IVeilingService _service;

        // Constructor injecteert de veilingservice
        public VeilingController(IVeilingService service)
        {
            _service = service;
        }

        // Start (of plan) een nieuwe veiling
        // Endpoint: POST /api/veiling/start
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartVeilingDto dto)
        {
            // Validatie: request body mag niet null zijn
            if (dto == null)
                return BadRequest(new { message = "Request body ontbreekt." });

            // Als er geen starttijd is opgegeven, gebruik standaard 09:00
            var startTijd = dto.StartTijd ?? new TimeSpan(9, 0, 0);

            // Huidige tijd in Nederlandse tijdzone
            var nowNl = NlTime.Now();

            // Geplande startdatum + starttijd (NL tijd)
            var geplandeStartNl = dto.Veildatum.Date + startTijd;

            // Controle: veiling mag niet in het verleden of te snel starten
            if (geplandeStartNl < nowNl.AddMinutes(1))
            {
                return BadRequest(new
                {
                    message = $"Starttijd ligt te vroeg. Kies een starttijd na {nowNl.AddMinutes(1):yyyy-MM-dd HH:mm}."
                });
            }

            // Start de veiling via de service
            var overzicht = await _service.StartVeilingAsync(
                dto.Veildatum,
                dto.LeverDatum,
                startTijd
            );

            // Succesvolle start: geef veilingoverzicht terug
            return Ok(overzicht);
        }

        // Haalt details van een specifieke veiling op
        // Endpoint: GET /api/veiling/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            // Service haalt veilingdetails op
            var details = await _service.GetDetailsAsync(id);

            return Ok(details);
        }

        // Pauzeert een lopende veiling
        // Endpoint: POST /api/veiling/{id}/pause
        [HttpPost("{id}/pause")]
        public async Task<IActionResult> Pause(int id)
        {
            // Pauzeer veiling via service
            await _service.PauseAsync(id);

            // Geen inhoud nodig bij succes
            return NoContent();
        }

        // Hervat een gepauzeerde veiling
        // Endpoint: POST /api/veiling/{id}/resume
        [HttpPost("{id}/resume")]
        public async Task<IActionResult> Resume(int id)
        {
            // Hervat veiling via service
            await _service.ResumeAsync(id);

            return NoContent();
        }

        // Stopt (afsluiten) van een veiling
        // Endpoint: POST /api/veiling/{id}/stop
        [HttpPost("{id}/stop")]
        public async Task<IActionResult> Stop(int id)
        {
            // Stop veiling via service
            await _service.StopAsync(id);

            return NoContent();
        }

        // Haalt alle beschikbare veilingdagen op
        // Endpoint: GET /api/veiling/dagen
        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetVeilingDagen()
        {
            // Service levert lijst met veilingdagen
            var dagen = await _service.GetVeilingDagenAsync();

            return Ok(dagen);
        }
    }
}
