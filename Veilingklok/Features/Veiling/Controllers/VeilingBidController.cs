using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/bids")]
public sealed class VeilingBidController : ControllerBase
{
    private readonly IBidService _service;

    public VeilingBidController(IBidService service)
    {
        _service = service;
    }

    // --------------------------------------------------------------------
    // 1. Koper plaatst bod
    // POST: /api/veilingen/{veilingId}/bids
    // --------------------------------------------------------------------
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Koper))]
    [ProducesResponseType(typeof(BidDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PlaceBid(
        int veilingId,
        [FromBody] PlaceBidRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var koperId))
            return Unauthorized("UserId ontbreekt.");

        var result = await _service.PlaceBidAsync(koperId, veilingId, dto);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetBidsForVeiling), new { veilingId }, result.Value);
    }

    // --------------------------------------------------------------------
    // 2. Alle biedingen voor veiling
    // GET: /api/veilingen/{veilingId}/bids
    // --------------------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> GetBidsForVeiling(int veilingId)
    {
        var result = await _service.GetForVeilingAsync(veilingId);
        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // 3. Biedingen voor specifiek lot
    // GET: /api/veilingen/{veilingId}/bids/lot/{veilingProductId}
    // --------------------------------------------------------------------
    [HttpGet("lot/{veilingProductId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBidsForLot(int veilingId, int veilingProductId)
    {
        var result = await _service.GetForVeilingProductAsync(veilingId, veilingProductId);
        return Ok(result.Value);
    }
}
