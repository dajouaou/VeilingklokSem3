using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;

namespace Veilingklok.Features.AanvoerderDashboard.Controllers;

[ApiController]
[Route("api/aanvoerder/dashboard")]
[Authorize(Roles = "Aanvoerder")]
public class AanvoerderDashboardController : ControllerBase
{
    private readonly IAanvoerderDashboardService _service;

    public AanvoerderDashboardController(IAanvoerderDashboardService service)
    {
        _service = service;
    }

    private int GetGebruikerId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idStr))
            throw new ArgumentException("Gebruiker-id ontbreekt in token.");
        return int.Parse(idStr);
    }

    // GET api/aanvoerder/dashboard/aanmeldingen?veildatum=2025-12-05
    [HttpGet("aanmeldingen")]
    public async Task<ActionResult<List<AanmeldingListItemDto>>> GetAanmeldingen([FromQuery] DateTime? veildatum)
    {
        var gebruikerId = GetGebruikerId();
        var list = await _service.GetAanmeldingenAsync(gebruikerId, veildatum);
        return Ok(list);
    }

    // POST api/aanvoerder/dashboard/aanmeldingen
    [HttpPost("aanmeldingen")]
    public async Task<ActionResult<AanmeldingListItemDto>> CreateAanmelding([FromBody] AanmeldingCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var gebruikerId = GetGebruikerId();
        var created = await _service.CreateAanmeldingAsync(gebruikerId, dto);

        return CreatedAtAction(nameof(GetAanmeldingen),
            new { veildatum = created.Veildatum.Date },
            created);
    }

    // GET api/aanvoerder/dashboard/statistieken?veildatum=2025-12-05
    [HttpGet("statistieken")]
    public async Task<ActionResult<AanvoerderStatsDto>> GetStats([FromQuery] DateTime? veildatum)
    {
        var gebruikerId = GetGebruikerId();
        var dto = await _service.GetStatsAsync(gebruikerId, veildatum);
        return Ok(dto);
    }
}
