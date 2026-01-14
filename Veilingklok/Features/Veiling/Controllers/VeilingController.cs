using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Route("api/veiling")]
    [Authorize(Roles = "Veilingmeester")]
    // Controller voor acties van de veilingmeester
    public class VeilingController : ControllerBase
    {
        private readonly IVeilingService _service;

        // Injecteert de veilingservice
        public VeilingController(IVeilingService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        // Start een nieuwe veiling
        public async Task<IActionResult> Start([FromBody] StartVeilingDto dto)
        {
            var overzicht = await _service.StartVeilingAsync(dto.Veildatum, dto.LeverDatum, dto.StartTijd);
            return Ok(overzicht);
        }


        [HttpGet("{id}")]
        // Haalt details van een veiling op
        public async Task<IActionResult> GetDetails(int id)
        {
            return Ok(await _service.GetDetailsAsync(id));
        }

        [HttpPost("{id}/pause")]
        // Pauzeert een lopende veiling
        public async Task<IActionResult> Pause(int id)
        {
            await _service.PauseAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/resume")]
        // Hervat een gepauzeerde veiling
        public async Task<IActionResult> Resume(int id)
        {
            await _service.ResumeAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/stop")]
        // Stopt en sluit een veiling af
        public async Task<IActionResult> Stop(int id)
        {
            await _service.StopAsync(id);
            return NoContent();
        }

        [HttpGet("dagen")]
        // Haalt alle veildagen op
        public async Task<ActionResult<List<string>>> GetVeilingDagen()
        {
            var dagen = await _service.GetVeilingDagenAsync();
            return Ok(dagen);
        }
    }
}
