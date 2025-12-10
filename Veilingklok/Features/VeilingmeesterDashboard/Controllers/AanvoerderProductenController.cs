using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.VeilingmeesterDashboard.Services;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Authorize(Roles = "Veilingmeester")]
    [Route("api/aanvoerder")]
    public class AanvoerderProductController : ControllerBase
    {
        private readonly IPlanningService _service;

        public AanvoerderProductController(IPlanningService service)
        {
            _service = service;
        }

        [HttpGet("producten")]
        public async Task<IActionResult> GetProducten([FromQuery] DateTime? veildatum)
        {
            var data = await _service.GetProductenPerVeildatumAsync(veildatum);
            return Ok(data);
        }
    }
}
