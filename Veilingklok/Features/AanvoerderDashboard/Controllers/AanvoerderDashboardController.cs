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
    [HttpPost("veildagen")]
    public async Task<ActionResult> CreateVeildag([FromBody] DateTime datum)
    {
        if (datum.Date < DateTime.Today)
            return BadRequest("Datum mag niet in het verleden liggen.");

        var bestaat = await _db.Veildagen.AnyAsync(v => v.Datum == datum.Date);
        if (bestaat)
            return BadRequest("Deze veildatum bestaat al.");

        _db.Veildagen.Add(new Veildag { Datum = datum.Date });
        await _db.SaveChangesAsync();

        return Ok();
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
