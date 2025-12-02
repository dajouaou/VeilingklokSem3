using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/clock")]
public sealed class ClockController : ControllerBase
{
    private readonly IClockService _clockService;

    public ClockController(IClockService clockService)
    {
        _clockService = clockService;
    }

    // GET: /api/veilingen/3/clock/state
    [HttpGet("state")]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)},{nameof(UserRole.Koper)}")]
    public async Task<IActionResult> GetClockState(int veilingId)
    {
        var result = await _clockService.GetStateAsync(veilingId);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }
}