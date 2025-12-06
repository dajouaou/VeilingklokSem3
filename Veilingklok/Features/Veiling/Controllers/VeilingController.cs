using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.Veiling.Services;

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

    // actieve product ophalen
    [HttpGet("{veilingId:int}/current")]
    public async Task<IActionResult> GetCurrentProduct(int veilingId)
    {
        var result = await _service.GetCurrentProductAsync(veilingId);
        return result.Success ? Ok(result.Value) : BadRequest(result.Error);
    }

    // queue ophalen
    [HttpGet("{veilingId:int}/queue")]
    public async Task<IActionResult> GetQueue(int veilingId)
    {
        var result = await _service.GetQueueAsync(veilingId);
        return result.Success ? Ok(result.Value) : BadRequest(result.Error);
    }

    // bod plaatsen
    [HttpPost("{veilingId:int}/bids")]
    public async Task<IActionResult> PlaceBid(int veilingId, int koperId, decimal amount)
    {
        var result = await _service.PlaceBidAsync(veilingId, koperId, amount);
        return result.Success ? Ok(result.Value) : BadRequest(result.Error);
    }
}