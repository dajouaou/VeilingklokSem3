using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Dtos;

namespace Veilingklok.Features.AanvoerderDashboard.Controllers;

[ApiController]
[Route("api/dashboard/aanvoerders")] // ✔ duidelijke feature-route
[Authorize] // ✔ alles achter auth
public class AanvoerderController : ControllerBase
{
    private readonly IAanvoerderService _service;

    public AanvoerderController(IAanvoerderService service)
    {
        _service = service;
    }

    // --------------------------------------------------------------------
    // GET ALL
    // Admin + Veilingmeester → mogen dashboard data zien
    // --------------------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Veilingmeester)}")]
    [ProducesResponseType(typeof(List<AanvoerderDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (result.IsFailure)
            return BadRequest(result.Error); // ✔ 400 bij service fout

        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // GET BY ID
    // --------------------------------------------------------------------
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Veilingmeester)}")]
    [ProducesResponseType(typeof(AanvoerderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);

        return result.IsFailure
            ? NotFound(result.Error)
            : Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // CREATE
    // Alleen Admin → koppelt token userId aan nieuwe aanvoerder
    // --------------------------------------------------------------------
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(AanvoerderDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateAanvoerderDto dto)
    {
        // ✔ userId uit JWT claim halen (niet uit DTO)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized("Kon gebruiker niet bepalen uit token.");

        int gebruikerId = int.Parse(userIdClaim);

        var result = await _service.CreateAsync(dto, gebruikerId);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value);
    }

    // --------------------------------------------------------------------
    // UPDATE
    // Admin + Veilingmeester
    // --------------------------------------------------------------------
    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Veilingmeester)}")]
    [ProducesResponseType(typeof(AanvoerderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAanvoerderDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (result.IsFailure)
        {
            if (result.Error.Contains("niet gevonden"))
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // DELETE
    // Alleen Admin
    // --------------------------------------------------------------------
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (result.IsFailure)
        {
            if (result.Error.Contains("niet gevonden"))
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
