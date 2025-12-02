using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingProduct.Dtos;

namespace Veilingklok.Features.VeilingProduct.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/loten")]
public sealed class VeilingProductController : ControllerBase
{
    private readonly IVeilingProductService _service;

    public VeilingProductController(IVeilingProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int veilingId)
        => Ok((await _service.GetByVeilingAsync(veilingId)).Value);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.IsFailure ? NotFound(r.Error) : Ok(r.Value);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)}, {nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Create(int veilingId, CreateVeilingProductDto dto)
    {
        dto.VeilingId = veilingId;

        var r = await _service.CreateAsync(dto);

        return r.IsFailure
            ? BadRequest(r.Error)
            : CreatedAtAction(nameof(Get), new { id = r.Value.Id }, r.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)}, {nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(int id, UpdateVeilingProductDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r.IsFailure ? NotFound(r.Error) : Ok(r.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)}, {nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.IsFailure ? NotFound(r.Error) : NoContent();
    }

    // Dashboard functionaliteit
    [HttpPost("{id:int}/active")]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)}, {nameof(UserRole.Admin)}")]
    public async Task<IActionResult> SetActive(int id)
        => Ok((await _service.SetActiveAsync(id)).Value);

    [HttpPost("{id:int}/sold/{koperId:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)}, {nameof(UserRole.Admin)}")]
    public async Task<IActionResult> MarkAsSold(int id, int koperId)
        => Ok((await _service.MarkAsSoldAsync(id, koperId)).Value);
}
