using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/bids")]
public sealed class BidController : ControllerBase
{
    private readonly IBidService _service;

    public BidController(IBidService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Koper))]
    public async Task<IActionResult> PlaceBid(int veilingId, BidCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _service.PlaceBidAsync(veilingId, userId, dto);

        return result.IsFailure
            ? BadRequest(result.Error)
            : Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Veilingmeester)}")]
    public async Task<IActionResult> GetList(int veilingId)
    {
        var result = await _service.GetBidsForVeilingAsync(veilingId);
        return Ok(result.Value);
    }
}