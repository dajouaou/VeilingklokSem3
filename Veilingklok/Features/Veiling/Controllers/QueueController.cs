using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/queue")]
public sealed class QueueController : ControllerBase
{
    private readonly IQueueService _queue;

    public QueueController(IQueueService queue)
    {
        _queue = queue;
    }

    // GET: /api/veilingen/3/queue
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> GetQueue(int veilingId)
    {
        var result = await _queue.GetQueueAsync(veilingId);
        return Ok(result.Value);
    }

    // POST: /api/veilingen/3/queue/reorder
    [HttpPost("reorder")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Reorder(int veilingId, QueueReorderDto dto)
    {
        var result = await _queue.ReorderAsync(veilingId, dto);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }

    // POST: /api/veilingen/3/queue/next
    [HttpPost("next")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> NextLot(int veilingId)
    {
        var result = await _queue.NextAsync(veilingId);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }
}