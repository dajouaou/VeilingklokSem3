using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Features.AanvoerderDashboard.Services;


namespace Veilingklok.Features.AanvoerderDashboard.Controllers;

[ApiController]
[Route("api/aanvoerder/dashboard")]
[Authorize(Roles = "Aanvoerder")]
// Controller voor alle aanvoerder dashboard acties
public class AanvoerderDashboardController : ControllerBase
{
    private readonly MyContext _db;
    private readonly IAanvoerderDashboardService _service;
    private readonly IWebHostEnvironment _env;
    private readonly IBlobImageService _blob;

    // Injecteert service, database en hosting info
    public AanvoerderDashboardController(
        IAanvoerderDashboardService service,
        MyContext db,
        IWebHostEnvironment env,
        IBlobImageService blob)
    {
        _service = service;
        _db = db;
        _env = env;
        _blob = blob;
    }

    // Haalt gebruiker-id uit de JWT token
    private bool TryGetGebruikerId(out int gebruikerId)
    {
        gebruikerId = 0;

        var idStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("nameid") ??
            User.FindFirstValue("sub");

        return int.TryParse(idStr, out gebruikerId);
    }

    // Geeft een standaard unauthorized response
    private ActionResult UnauthorizedUserId()
        => Unauthorized("Geen geldige gebruiker-id in token.");

    [HttpGet("aanmeldingen")]
    // Haalt alle aanmeldingen op voor deze aanvoerder
    public async Task<ActionResult<List<AanmeldingListItemDto>>> GetAanmeldingen([FromQuery] DateTime? leverdatum)
    {
        if (!TryGetGebruikerId(out var gebruikerId))
            return UnauthorizedUserId();

        try
        {
            var result = await _service.GetAanmeldingenAsync(gebruikerId, leverdatum);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("aanmeldingen")]
    public async Task<ActionResult<AanmeldingListItemDto>> CreateAanmelding([FromForm] AanmeldingCreateDto dto)
    {
        if (!TryGetGebruikerId(out var gebruikerId))
            return UnauthorizedUserId();

        try
        {
            string? fotoUrl = null;

            if (dto.Foto != null && dto.Foto.Length > 0)
            {
                fotoUrl = await _blob.UploadAsync(dto.Foto, HttpContext.RequestAborted);
            }

            var result = await _service.CreateAanmeldingAsync(gebruikerId, dto, fotoUrl);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("aanmeldingen/{id}")]
    public async Task<ActionResult<AanmeldingListItemDto>> UpdateAanmelding(int id, [FromForm] AanmeldingUpdateDto dto)
    {
        if (!TryGetGebruikerId(out var gebruikerId))
            return UnauthorizedUserId();

        try
        {
            string? fotoUrl = null;

            if (dto.Foto != null && dto.Foto.Length > 0)
            {
                fotoUrl = await _blob.UploadAsync(dto.Foto, HttpContext.RequestAborted);
            }

            var updated = await _service.UpdateAanmeldingAsync(gebruikerId, id, dto, fotoUrl);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            if (ex.Message.Contains("niet gevonden", StringComparison.OrdinalIgnoreCase))
                return NotFound(ex.Message);

            return BadRequest(ex.Message);
        }
    }


    [HttpDelete("aanmeldingen/{id}")]
    // Verwijdert een aanmelding
    public async Task<ActionResult> DeleteAanmelding(int id)
    {
        if (!TryGetGebruikerId(out var gebruikerId))
            return UnauthorizedUserId();

        try
        {
            await _service.DeleteAanmeldingAsync(gebruikerId, id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            if (ex.Message.Contains("niet gevonden", StringComparison.OrdinalIgnoreCase))
                return NotFound(ex.Message);

            return BadRequest(ex.Message);
        }
    }

    [HttpGet("statistieken")]
    // Haalt statistieken op voor de aanvoerder
    public async Task<ActionResult<AanvoerderStatsDto>> GetStats([FromQuery] DateTime? leverdatum)
    {
        if (!TryGetGebruikerId(out var gebruikerId))
            return UnauthorizedUserId();

        try
        {
            var stats = await _service.GetStatsAsync(gebruikerId, leverdatum);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("veildagen")]
    // Haalt alle veildagen op
    public async Task<ActionResult<List<string>>> GetVeildagen()
    {
        var dagen = await _db.Veildagen
            .OrderBy(v => v.Datum)
            .Select(v => v.Datum.ToString("yyyy-MM-dd"))
            .ToListAsync();

        return Ok(dagen);
    }
}
