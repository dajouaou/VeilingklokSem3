using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/lot")]
public sealed class LotController : ControllerBase
{
    private readonly IVeilingLotService _lot;

    public LotController(IVeilingLotService lot)
    {
        _lot = lot;
    }

    [HttpPost("start/{veilingProductId:int}")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> StartLot(int veilingId, int veilingProductId)
    {
        await _lot.StartLotAsync(veilingId, veilingProductId);
        return NoContent();
    }

    [HttpPost("finish/{veilingProductId:int}")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> FinishLot(int veilingId, int veilingProductId)
    {
        await _lot.FinishLotAsync(veilingId, veilingProductId);
        return NoContent();
    }

    [HttpPost("sold/{veilingProductId:int}")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Sold(int veilingId, int veilingProductId, MarkLotSoldDto dto)
    {
        await _lot.MarkSoldAsync(veilingId, veilingProductId, dto);
        return NoContent();
    }

    [HttpPost("unsold/{veilingProductId:int}")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Unsold(int veilingId, int veilingProductId)
    {
        await _lot.MarkUnsoldAsync(veilingId, veilingProductId);
        return NoContent();
    }
}