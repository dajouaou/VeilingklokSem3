using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Route("api/veilingmeester/planning")]
    public class PlanningController : ControllerBase
    {
        private readonly IPlanningService _service;

        public PlanningController(IPlanningService service)
        {
            _service = service;
        }

        [HttpGet("producten")]
        public async Task<IActionResult> GetProducten()
        {
            var producten = await _service.GetBeschikbareProductenAsync();
            return Ok(producten);
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartVeiling([FromBody] StartVeilingRequestDto dto)
        {
            var result = await _service.StartVeilingMetPlanningAsync(dto);
            return Ok(result);
        }
    }
}
