// Veilingklok/Features/Veiling/Controllers/VeilingFacadeController.cs
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.Veiling.Services;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen")]
public sealed class VeilingFacadeController : ControllerBase
{
    private readonly IVeilingPublicService _public;
    private readonly IBiddingService _bids;

    public VeilingFacadeController(IVeilingPublicService @public, IBiddingService bids)
    {
        _public = @public;
        _bids = bids;
    }

    private int GetGebruikerId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Geen geldige gebruiker-id in token.");
        return id;
    }

    [AllowAnonymous]
    [HttpGet("public/leverdagen")]
    [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicLeverdagen()
        => this.ToActionResult(await _public.GetBeschikbareLeverdagenAsync());

    [AllowAnonymous]
    [HttpGet("{veilingId:int}/public")]
    [ProducesResponseType(typeof(PublicVeilingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublic(int veilingId)
        => this.ToActionResult(await _public.LoadPublicAsync(veilingId));

    [Authorize(Roles = "Koper")]
    [HttpPost("{veilingId:int}/bids")]
    [ProducesResponseType(typeof(BidResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PlaceBid(int veilingId, [FromBody] PlaceBidRequestDto dto)
        => this.ToActionResult(await _bids.PlaceBidAsync(veilingId, GetGebruikerId(), dto.Price));
}
