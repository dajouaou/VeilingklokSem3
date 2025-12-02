using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Producten.Dtos;

namespace Veilingklok.Features.Producten.Controllers;

[ApiController]
[Route("api/producten")]
public sealed class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    // ============================================================
    // GET ALL (Summary List)
    // ============================================================
    // GET /api/producten
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result.Value);
    }

    // ============================================================
    // GET DETAILS BY ID
    // ============================================================
    // GET /api/producten/{id}
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetOne(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
    }

    // ============================================================
    // GET BY AANVOERDER (role-restricted)
    // ============================================================
    // GET /api/producten/aanvoerder/{aanvoerderId}
    [HttpGet("aanvoerder/{aanvoerderId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Aanvoerder)}")]
    public async Task<IActionResult> GetByAanvoerder(int aanvoerderId)
    {
        var result = await _service.GetByAanvoerderAsync(aanvoerderId);
        return Ok(result.Value);
    }

    // ============================================================
    // SEARCH + PAGINATION
    // ============================================================
    // POST /api/producten/search
    [HttpPost("search")]
    [Authorize]
    public async Task<IActionResult> Search([FromBody] SearchProductDto dto)
    {
        var result = await _service.SearchAsync(dto);
        return Ok(result.Value);
    }

    // ============================================================
    // CREATE
    // ============================================================
    // POST /api/producten
    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Aanvoerder)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetOne), new { id = result.Value.Id }, result.Value);
    }

    // ============================================================
    // UPDATE
    // ============================================================
    // PUT /api/producten/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Aanvoerder)},{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        return result.IsFailure
            ? Conflict(result.Error) // concurrency → 409
            : Ok(result.Value);
    }

    // ============================================================
    // SOFT DELETE
    // ============================================================
    // DELETE /api/producten/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var result = await _service.DeleteAsync(id);

        return result.IsFailure
            ? NotFound(result.Error)
            : NoContent();
    }

    // ============================================================
    // RESTORE
    // ============================================================
    // PATCH /api/producten/{id}/restore
    [HttpPatch("{id:int}/restore")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Restore(int id)
    {
        var result = await _service.RestoreAsync(id);

        return result.IsFailure
            ? NotFound(result.Error)
            : Ok(new { restored = true });
    }
}
