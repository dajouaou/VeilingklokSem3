using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

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

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartVeilingDto dto)
        {
            var overzicht = await _service.StartVeilingAsync(dto.Veildatum, dto.StartTijd);
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

        [HttpPost("{id}/bod")]
        public async Task<IActionResult> PlaatsBod(int id, [FromBody] BodPlaatsenDto dto)
        {
            // TODO: koperId uit token halen
            int koperId = 999;

            var bod = await _service.PlaatsBodAsync(id, dto, koperId);
            return Ok(bod);
        }

        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetVeilingDagen()
        {
            var dagen = await _service.GetVeilingDagenAsync();
            return Ok(dagen);
        }
    }
}
