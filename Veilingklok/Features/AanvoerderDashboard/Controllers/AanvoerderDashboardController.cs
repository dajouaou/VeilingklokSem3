using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Veilingklok.Features.AanvoerderDashboard.Controllers;

[ApiController]
[Route("api/aanvoerder/dashboard")]
[Authorize(Roles = "Aanvoerder")]
public class AanvoerderDashboardController : ControllerBase
{
    private readonly IAanvoerderDashboardService _service;
    private readonly MyContext _db;

    public AanvoerderDashboardController(IAanvoerderDashboardService service, MyContext db)
    {
        _service = service;
        _db = db;
    }

    private int GetGebruikerId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idStr ?? throw new ArgumentException("Geen gebruiker-id in token."));
    }

    [HttpGet("aanmeldingen")]
    public async Task<ActionResult<List<AanmeldingListItemDto>>> GetAanmeldingen([FromQuery] DateTime? veildatum)
    {
        var gebruikerId = GetGebruikerId();
        return Ok(await _service.GetAanmeldingenAsync(gebruikerId, veildatum));
    }

    [HttpPost("aanmeldingen")]
    public async Task<ActionResult<AanmeldingListItemDto>> CreateAanmelding([FromBody] AanmeldingCreateDto dto)
    {
        var gebruikerId = GetGebruikerId();
        var created = await _service.CreateAanmeldingAsync(gebruikerId, dto);
        return Ok(created);
    }

    // ✅ FIXED: UPDATE GEBRUIKT NU AANMELDINGUPDATEDTO
    [HttpPut("aanmeldingen/{id}")]
    public async Task<ActionResult<AanmeldingListItemDto>> UpdateAanmelding(int id, [FromBody] AanmeldingUpdateDto dto)
    {
        var gebruikerId = GetGebruikerId();
        var updated = await _service.UpdateAanmeldingAsync(gebruikerId, id, dto);
        return Ok(updated);
    }

    [HttpDelete("aanmeldingen/{id}")]
    public async Task<ActionResult> DeleteAanmelding(int id)
    {
        var gebruikerId = GetGebruikerId();
        await _service.DeleteAanmeldingAsync(gebruikerId, id);
        return NoContent();
    }

    [HttpGet("statistieken")]
    public async Task<ActionResult<AanvoerderStatsDto>> GetStats([FromQuery] DateTime? veildatum)
    {
        var gebruikerId = GetGebruikerId();
        return Ok(await _service.GetStatsAsync(gebruikerId, veildatum));
    }

    [HttpGet("veildagen")]
    public async Task<ActionResult<List<string>>> GetVeildagen()
    {
        var dagen = await _db.Veildagen
            .OrderBy(v => v.Datum)
            .Select(v => v.Datum.ToString("yyyy-MM-dd"))
            .ToListAsync();

        return Ok(dagen);
    }
}
