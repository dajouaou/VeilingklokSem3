using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;

namespace Veilingklok.Features.Clock.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/clock")]
public sealed class ClockController : ControllerBase
{
    private readonly IClockService _clock;

    public ClockController(IClockService clock)
    {
        _clock = clock;
    }

    [HttpPost("start/{veilingProductId:int}")]
    public async Task<IActionResult> Start(int veilingProductId)
        => Ok((await _clock.StartClockAsync(veilingProductId)).Value);
}
