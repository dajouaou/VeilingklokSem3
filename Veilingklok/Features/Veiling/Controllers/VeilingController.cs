using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Route("api/veiling")]
    [Authorize(Roles = "Veilingmeester")]
    // Alleen gebruikers met rol "Veilingmeester" mogen deze endpoints gebruiken
    public class VeilingController : ControllerBase
    {
        private readonly IVeilingService _service;

        public VeilingController(IVeilingService service)
        {
            _service = service; // businesslogica zit in de service, controller blijft dun
        }

        // Start een nieuwe veiling op basis van datum en starttijd
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartVeilingDto dto)
        {
            var overzicht = await _service.StartVeilingAsync(
                dto.Veildatum,
                dto.LeverDatum,
                dto.StartTijd
            );

            return Ok(overzicht); // geeft direct het veilingoverzicht terug
        }

        // Haalt details van een specifieke veiling op
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            return Ok(await _service.GetDetailsAsync(id));
        }

        // Pauzeert een lopende veiling
        [HttpPost("{id}/pause")]
        public async Task<IActionResult> Pause(int id)
        {
            await _service.PauseAsync(id);
            return NoContent(); // geen response-body nodig
        }

        // Hervat een gepauzeerde veiling
        [HttpPost("{id}/resume")]
        public async Task<IActionResult> Resume(int id)
        {
            await _service.ResumeAsync(id);
            return NoContent();
        }

        // Stopt en sluit een veiling definitief af
        [HttpPost("{id}/stop")]
        public async Task<IActionResult> Stop(int id)
        {
            await _service.StopAsync(id);
            return NoContent();
        }

        // Geeft alle veildagen terug waarvoor een veiling bestaat
        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetVeilingDagen()
        {
            var dagen = await _service.GetVeilingDagenAsync();
            return Ok(dagen);
        }
    }
}
