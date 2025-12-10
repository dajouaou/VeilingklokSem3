using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Authorize(Roles = "Veilingmeester")]
    [Route("api/veilingen")]
    public class VeilingenController : ControllerBase
    {
        private readonly IPlanningService _planning;

        public VeilingenController(IPlanningService planning)
        {
            _planning = planning;
        }

        // GET /api/veilingen
        [HttpGet]
        public async Task<ActionResult<List<VeilingPlanningDto>>> GetAll()
        {
            var result = await _planning.GetGeplandeVeilingenAsync();
            return Ok(result);
        }

        // POST /api/veilingen
        [HttpPost]
        public async Task<ActionResult<VeilingPlanningDto>> Create([FromBody] VeilingCreateDto dto)
        {
            var result = await _planning.CreateVeilingAsync(dto);
            return Ok(result);
        }

        // PUT /api/veilingen/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<VeilingPlanningDto>> Update(int id, [FromBody] VeilingCreateDto dto)
        {
            var result = await _planning.UpdateVeilingAsync(id, dto);
            return Ok(result);
        }

        // DELETE /api/veilingen/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _planning.DeleteVeilingAsync(id);
            return NoContent();
        }

        // POST /api/veilingen/{id}/start
        [HttpPost("{id}/start")]
        public async Task<ActionResult<VeilingPlanningDto>> Start(int id)
        {
            var result = await _planning.StartGeplandeVeilingAsync(id);
            return Ok(result);
        }
    }
}
