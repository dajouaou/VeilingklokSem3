using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Controllers;

[ApiController]
[Route("api/veilingen")]
public sealed class VeilingController : ControllerBase
{
    private readonly IVeilingService _service;

    public VeilingController(IVeilingService service)
    {
        _service = service;
    }

    // GET ALL
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
        => Ok((await _service.GetAllAsync()).Value);

    // GET ONE
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.IsFailure ? NotFound(r.Error) : Ok(r.Value);
    }

    // CREATE
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Create(CreateVeilingDto dto)
    {
        var r = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = r.Value.Id }, r.Value);
    }

    // UPDATE
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Update(int id, UpdateVeilingDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r.IsFailure ? NotFound(r.Error) : Ok(r.Value);
    }

    // DELETE
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.IsFailure ? NotFound(r.Error) : NoContent();
    }

    // START VEILING
    [HttpPost("{id:int}/start")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Start(int id, StartVeilingDto dto)
    {
        var r = await _service.StartVeilingAsync(id, dto);
        return r.IsFailure ? BadRequest(r.Error) : Ok(r.Value);
    }

    // SET CURRENT LOT
    [HttpPost("{id:int}/current-lot")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> SetCurrentLot(int id, ChangeCurrentLotDto dto)
    {
        var r = await _service.SetCurrentLotAsync(id, dto);
        return r.IsFailure ? BadRequest(r.Error) : Ok(r.Value);
    }

    // STOP
    [HttpPost("{id:int}/stop")]
    [Authorize(Roles = nameof(UserRole.Veilingmeester))]
    public async Task<IActionResult> Stop(int id)
    {
        var r = await _service.StopVeilingAsync(id);
        return r.IsFailure ? BadRequest(r.Error) : Ok(r.Value);
    }
}
