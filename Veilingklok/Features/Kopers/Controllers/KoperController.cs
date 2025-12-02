using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Koper.Dtos;

namespace Veilingklok.Features.Koper.Controllers;

[ApiController]
[Route("api/kopers")]
public sealed class KoperController : ControllerBase
{
    private readonly IKoperService _service;

    public KoperController(IKoperService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Veilingmeester)}")]
    [ProducesResponseType(typeof(List<KoperListItemDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Authorize(Roles = nameof(UserRole.Koper))]
    [ProducesResponseType(typeof(KoperDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.GetByUserIdAsync(userId);
        return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(KoperDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateKoperDto dto)
    {
        var result = await _service.CreateByAdminAsync(dto);
        return Created("", result.Value);
    }

    [HttpPut("me")]
    [Authorize(Roles = nameof(UserRole.Koper))]
    [ProducesResponseType(typeof(KoperDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromBody] UpdateKoperDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.UpdateByUserIdAsync(userId, dto);
        return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.IsFailure ? NotFound(result.Error) : NoContent();
    }
}
