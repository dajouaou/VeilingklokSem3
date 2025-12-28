// Veilingklok/Features/Veilingmeester/Controllers/VeilingmeesterPlanningController.cs
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.Veiling.Services;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.Veilingmeester.Controllers;

[Authorize(Roles = "Veilingmeester")]
[ApiController]
[Route("api/veilingmeester/planning")]
public sealed class VeilingmeesterPlanningController : ControllerBase
{
    private readonly IVeilingPlanningService _planning;

    public VeilingmeesterPlanningController(IVeilingPlanningService planning)
    {
        _planning = planning;
    }

    private int GetActorId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Geen geldige gebruiker-id in token.");
        return id;
    }

    [HttpPost("create-from-leverdatum")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFromLeverdatum([FromBody] CreateVeilingFromLeverdatumDto dto)
        => this.ToActionResult(await _planning.CreateVeilingFromLeverdatumAsync(dto, GetActorId()));

    [HttpGet("veildagen")]
    [ProducesResponseType(typeof(System.Collections.Generic.List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVeildagen()
        => this.ToActionResult(await _planning.GetVeildagenAsync());

    [HttpGet("aanmeldingen")]
    [ProducesResponseType(typeof(System.Collections.Generic.List<VeilingPlanningAanmeldingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAanmeldingen([FromQuery] string leverdatum)
        => this.ToActionResult(await _planning.GetAanmeldingenAsync(leverdatum));

    [HttpPost("plan")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PlanVeiling([FromBody] PlanVeilingRequestDto dto)
        => this.ToActionResult(await _planning.PlanVeilingAsync(dto, GetActorId()));

    [HttpGet("geplande")]
    [ProducesResponseType(typeof(System.Collections.Generic.List<GeplandeVeilingListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGeplande()
        => this.ToActionResult(await _planning.GetGeplandeAsync());

    [HttpGet("volgende")]
    [ProducesResponseType(typeof(GeplandeVeilingListItemDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVolgende()
        => this.ToActionResult(await _planning.GetVolgendeGeplandeAsync());
}
