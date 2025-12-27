//dit aanpassen
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers
{
    [ApiController]
    [Authorize(Roles = "Veilingmeester")]
    [Route("api/veilingmeester/dashboard")]
    public class VeilingmeesterDashboardController : ControllerBase
    {
        private readonly IVeilingService _service;

        public VeilingmeesterDashboardController(IVeilingService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VeilingmeesterDashboardDto>> GetDashboard(int id)
        {
            var overzicht = await _service.GetDetailsAsync(id);

            return Ok(new VeilingmeesterDashboardDto
            {
                Overzicht = overzicht ?? new(),
                Biedingen = new(),      // evt. later vullen met echte biedingen
                AuditEvents = new()
            });
        }

    }
}
