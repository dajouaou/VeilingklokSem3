using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/bids")]
public sealed class BidController : ControllerBase
{
    private readonly IVeilingBidService _bids;

    public BidController(IVeilingBidService bids)
    {
        _bids = bids;
    }

    // ---------------------------------------------------------
    // POST: koper plaatst bod
    // ---------------------------------------------------------
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Koper))]
    [ProducesResponseType(typeof(BidDto), 201)]
    public async Task<IActionResult> PlaceBid(int veilingId, PlaceBidDto dto)
    {
        var gebruikerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        dto.GebruikerId = gebruikerId;

        var result = await _bids.PlaceBidAsync(veilingId, dto);
        return Created($"api/veilingen/{veilingId}/bids", result);
    }

    // ---------------------------------------------------------
    // GET ALL bids for veiling
    // ---------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(List<BidDto>), 200)]
    public async Task<IActionResult> GetBids(int veilingId)
        => Ok(await _bids.GetBidsForVeilingAsync(veilingId));

    // ---------------------------------------------------------
    // GET bids for lot
    // ---------------------------------------------------------
    [HttpGet("lot/{veilingProductId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<BidDto>), 200)]
    public async Task<IActionResult> GetLotBids(int veilingId, int veilingProductId)
        => Ok(await _bids.GetBidsForLotAsync(veilingId, veilingProductId));
}
