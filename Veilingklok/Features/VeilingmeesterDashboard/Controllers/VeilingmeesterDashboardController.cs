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
    // Controller die het dashboard-overzicht voor de veilingmeester teruggeeft
    public class VeilingmeesterDashboardController : ControllerBase
    {
        private readonly IVeilingService _service;

        // Injecteert de veilingservice
        public VeilingmeesterDashboardController(IVeilingService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        // Haalt dashboarddata op voor een specifieke veiling
        public async Task<ActionResult<VeilingmeesterDashboardDto>> GetDashboard(int id)
        {
            // Haalt het veilingoverzicht op via de service
            var overzicht = await _service.GetDetailsAsync(id);

            // Bouwt het dashboard DTO (overzicht + lege lijsten voor biedingen/audit)
            return Ok(new VeilingmeesterDashboardDto
            {
                Overzicht = overzicht ?? new(),
                Biedingen = new(),      // evt. later vullen met echte biedingen
                AuditEvents = new()
            });
        }

    }
}
