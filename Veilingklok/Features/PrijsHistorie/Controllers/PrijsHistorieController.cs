// /Features/PrijsHistorie/Controllers/PrijsHistorieController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.PrijsHistorie.Services;

namespace Veilingklok.Features.PrijsHistorie.Controllers
{
    [ApiController]
    [Authorize(Roles = "Koper,Aanvoerder,Veilingmeester")]
    [Route("api/prijshistorie")]
    public class PrijsHistorieController : ControllerBase
    {
        private readonly IPrijsHistorieService _service;

        public PrijsHistorieController(IPrijsHistorieService service)
        {
            _service = service;
        }

        // GET /api/prijshistorie?soort=Rozen&aanvoerderId=3
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string soort, [FromQuery] int? aanvoerderId)
        {
            if (string.IsNullOrWhiteSpace(soort))
                return BadRequest("soort is verplicht.");

            var data = await _service.GetPrijsHistorieAsync(soort.Trim(), aanvoerderId);
            return Ok(data);
        }
    }
}
