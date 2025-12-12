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
    private readonly MyContext _db;
    private readonly IAanvoerderDashboardService _service;
    private readonly IWebHostEnvironment _env;

    public AanvoerderDashboardController(IAanvoerderDashboardService service,MyContext db, IWebHostEnvironment env)
    {
        _service = service;
        _db = db;
        _env = env;
    }


    private int GetGebruikerId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idStr ?? throw new ArgumentException("Geen gebruiker-id in token."));
    }

    [HttpGet("aanmeldingen")]
    public async Task<ActionResult<List<AanmeldingListItemDto>>> GetAanmeldingen([FromQuery] DateTime? leverdatum)
    {
        var gebruikerId = GetGebruikerId();
        return Ok(await _service.GetAanmeldingenAsync(gebruikerId, leverdatum));
    }

    [HttpPost("aanmeldingen")]
    public async Task<ActionResult<AanmeldingListItemDto>> CreateAanmelding([FromForm] AanmeldingCreateDto dto)
    {
        var gebruikerId = GetGebruikerId();

        string? fotoPad = null;


        if (dto.Foto != null && dto.Foto.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Foto.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Foto.CopyToAsync(stream);
            }

            fotoPad = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

        }

        var result = await _service.CreateAanmeldingAsync(gebruikerId, dto, fotoPad);

        return Ok(result);
    }


    [HttpPut("aanmeldingen/{id}")]
    public async Task<ActionResult<AanmeldingListItemDto>> UpdateAanmelding(
        int id,
        [FromForm] AanmeldingUpdateDto dto)
    {
        var gebruikerId = GetGebruikerId();

        string? fotoPad = null;

        if (dto.Foto != null && dto.Foto.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Foto.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Foto.CopyToAsync(stream);
            }

            fotoPad = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        }

        var updated = await _service.UpdateAanmeldingAsync(gebruikerId, id, dto, fotoPad);

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
    public async Task<ActionResult<AanvoerderStatsDto>> GetStats([FromQuery] DateTime? leverdatum)
    {
        var gebruikerId = GetGebruikerId();
        return Ok(await _service.GetStatsAsync(gebruikerId, leverdatum));
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
