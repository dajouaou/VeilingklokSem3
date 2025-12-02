using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}")]
public sealed class DashboardController : ControllerBase
{
    private readonly IVeilingDashboardService _dashboard;

    public DashboardController(IVeilingDashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    // ---------------------------------------------------------
    // GET: /api/veilingen/3
    // ---------------------------------------------------------
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(VeilingDetailsDto), 200)]
    public async Task<IActionResult> GetDetails(int veilingId)
        => Ok(await _dashboard.GetDetailsAsync(veilingId));

    // ---------------------------------------------------------
    // GET: /api/veilingen/3/current
    // ---------------------------------------------------------
    [HttpGet("current")]
    [AllowAnonymous] // klok publiek
    [ProducesResponseType(typeof(CurrentLotDto), 200)]
    public async Task<IActionResult> GetCurrentLot(int veilingId)
        => Ok(await _dashboard.GetCurrentLotAsync(veilingId));

    // ---------------------------------------------------------
    // GET: /api/veilingen/3/queue
    // ---------------------------------------------------------
    [HttpGet("queue")]
    [Authorize]
    [ProducesResponseType(typeof(List<QueueGroupDto>), 200)]
    public async Task<IActionResult> GetQueue(int veilingId)
        => Ok(await _dashboard.GetQueueAsync(veilingId));

    // ---------------------------------------------------------
    // GET: /api/veilingen/3/status
    // ---------------------------------------------------------
    [HttpGet("status")]
    [Authorize]
    [ProducesResponseType(typeof(VeilingStatusDto), 200)]
    public async Task<IActionResult> GetStatus(int veilingId)
        => Ok(await _dashboard.GetStatusAsync(veilingId));
}
