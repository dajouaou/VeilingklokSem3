using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Services;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen")]
public class VeilingmeesterDashboardController : ControllerBase
{
    private readonly IVeilingmeesterDashboardService _service;
    public VeilingmeesterDashboardController(IVeilingmeesterDashboardService service) => _service = service;

    [HttpGet("{veilingId:int}")]
    public async Task<ActionResult<VeilingDetailsDto>> GetDetails(int veilingId)
    {
        var dto = await _service.GetVeilingDetailsAsync(veilingId);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("{veilingId:int}/current")]
    public async Task<ActionResult<CurrentLotDto>> GetCurrent(int veilingId)
    {
        var dto = await _service.GetCurrentLotAsync(veilingId);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("{veilingId:int}/queue")]
    public async Task<ActionResult<List<QueueGroupDto>>> GetQueue(int veilingId)
        => Ok(await _service.GetQueueAsync(veilingId));

    [HttpGet("{veilingId:int}/queue/all")]
    public async Task<ActionResult<List<QueueGroupDto>>> GetQueueAll(int veilingId)
        => Ok(await _service.GetQueueAllAsync(veilingId));

    [HttpGet("{veilingId:int}/bids")]
    public async Task<ActionResult<List<BidDto>>> GetBids(
        int veilingId,
        [FromQuery] int? veilingProductId = null)
        => Ok(await _service.GetBidsAsync(veilingId, veilingProductId));

    [HttpGet("{veilingId:int}/audit")]
    public async Task<ActionResult<List<AuditDto>>> GetAudit(int veilingId)
        => Ok(await _service.GetAuditAsync(veilingId));

    // start: CreatedAtAction + veilingId terug (ArgumentException => 400 via global handler)
    [HttpPost]
    public async Task<IActionResult> StartAuction([FromBody] StartAuctionRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = await _service.StartAuctionAsync(dto);
        return CreatedAtAction(nameof(GetDetails), new { veilingId = id }, new { veilingId = id });
    }

    [HttpPost("{veilingId:int}/pause")]
    public async Task<IActionResult> Pause(int veilingId)
    {
        await _service.PauseAuctionAsync(veilingId);
        return NoContent();
    }

    [HttpPost("{veilingId:int}/resume")]
    public async Task<IActionResult> Resume(int veilingId)
    {
        await _service.ResumeAuctionAsync(veilingId);
        return NoContent();
    }

    [HttpPost("{veilingId:int}/stop")]
    public async Task<IActionResult> Stop(int veilingId)
    {
        await _service.StopAuctionAsync(veilingId);
        return NoContent();
    }

    [HttpPost("{veilingId:int}/queue")]
    public async Task<IActionResult> AddQueueItem(int veilingId, [FromBody] AddQueueItemDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.AddQueueItemAsync(veilingId, dto);
        return NoContent();
    }

    [HttpPut("{veilingId:int}/queue/reorder")]
    public async Task<IActionResult> ReorderQueue(int veilingId, [FromBody] ReorderQueueDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.ReorderQueueAsync(veilingId, dto);
        return NoContent();
    }

    [HttpPost("{veilingId:int}/bids")]
    public async Task<ActionResult<BidDto>> PlaceBid(int veilingId, [FromBody] PlaceBidDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.PlaceBidAsync(veilingId, dto);
        return Ok(result);
    }
}
