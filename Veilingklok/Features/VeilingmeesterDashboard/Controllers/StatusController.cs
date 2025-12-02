using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/status")]
public sealed class StatusController : ControllerBase
{
    private readonly IVeilingStatusService _status;

    public StatusController(IVeilingStatusService status)
    {
        _status = status;
    }

    // START ----------------------------------------------------
    [HttpPost("start")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Start(int veilingId, StartVeilingDto dto)
    {
        await _status.StartAsync(veilingId, dto);
        return NoContent();
    }

    // PAUSE ----------------------------------------------------
    [HttpPost("pause")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Pause(int veilingId)
    {
        await _status.PauseAsync(veilingId);
        return NoContent();
    }

    // RESUME ---------------------------------------------------
    [HttpPost("resume")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Resume(int veilingId)
    {
        await _status.ResumeAsync(veilingId);
        return NoContent();
    }

    // STOP -----------------------------------------------------
    [HttpPost("stop")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Stop(int veilingId)
    {
        await _status.StopAsync(veilingId);
        return NoContent();
    }
}