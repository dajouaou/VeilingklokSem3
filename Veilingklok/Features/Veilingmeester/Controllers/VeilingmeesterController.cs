using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veilingmeester.Dtos;

namespace Veilingklok.Features.Veilingmeester.Controllers;

[ApiController]
[Route("api/veilingmeesters")]
public sealed class VeilingmeesterController : ControllerBase
{
    private readonly IVeilingmeesterService _service;

    public VeilingmeesterController(IVeilingmeesterService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> GetAll()
        => Ok((await _service.GetAllAsync()).Value);

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.IsFailure ? NotFound(r.Error) : Ok(r.Value);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Create(CreateVeilingmeesterDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = r.Value.Id }, r.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Update(int id, UpdateVeilingmeesterDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r.IsFailure ? NotFound(r.Error) : Ok(r.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.IsFailure ? NotFound(r.Error) : NoContent();
    }
}