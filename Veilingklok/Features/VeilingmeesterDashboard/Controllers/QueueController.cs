using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/queue")]
public sealed class QueueController : ControllerBase
{
    private readonly IVeilingQueueService _queue;

    public QueueController(IVeilingQueueService queue)
    {
        _queue = queue;
    }

    // ADD ------------------------------------------------------
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Add(int veilingId, AddQueueItemDto dto)
    {
        await _queue.AddAsync(veilingId, dto);
        return NoContent();
    }

    // REORDER --------------------------------------------------
    [HttpPut("reorder")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Reorder(int veilingId, ReorderQueueDto dto)
    {
        await _queue.ReorderAsync(veilingId, dto);
        return NoContent();
    }

    // NEXT -----------------------------------------------------
    [HttpPost("next")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Next(int veilingId)
    {
        await _queue.MoveNextAsync(veilingId);
        return NoContent();
    }
}