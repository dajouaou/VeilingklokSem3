using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Home.Controllers
{
    [ApiController]
    [Authorize(Roles = "Koper")]
    [Route("api/bod")]
    public class BodController : ControllerBase
    {
        private readonly IVeilingService _service;

        public BodController(IVeilingService service)
        {
            _service = service;
        }

        [HttpPost("{veilingId}")]
        public async Task<IActionResult> PlaatsBod(
            int veilingId,
            [FromBody] BodPlaatsenDto dto)
        {
            int koperId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var bod = await _service.PlaatsBodAsync(veilingId, dto, koperId);
            return Ok(bod);
        }
    }
}
