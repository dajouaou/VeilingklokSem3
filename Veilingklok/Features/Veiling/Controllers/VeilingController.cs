// src/Features/Veiling/Controllers/VeilingController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.Veiling;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen")]
public class VeilingController : ControllerBase
{
    private readonly IVeilingService _service;

    public VeilingController(IVeilingService service)
    {
        _service = service;
    }

    [HttpGet("{veilingId:int}/public")]
    public async Task<IActionResult> GetPublic(int veilingId)
    {
        var result = await _service.LoadPublicAsync(veilingId);
        if (!result.Success)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("{veilingId:int}/bids")]
    public async Task<IActionResult> PlaceBid(int veilingId, [FromQuery] int koperId)
    {
        var result = await _service.PlaceBidAsync(veilingId, koperId);
        if (!result.Success)
            return BadRequest(result.Error);
        return Ok(result.Value);
    }
}