using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.VM.Services;
using Veilingklok.Features.VM.Dtos;

namespace Veilingklok.Features.VM;

[ApiController]
[Route("api/veilingen/{veilingId:int}/vm")]
public sealed class VMController : ControllerBase
{
    private readonly IVMService _service;

    public VMController(IVMService service)
    {
        _service = service;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(int veilingId)
    {
        var result = await _service.StartVeilingAsync(veilingId);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(VMVeilingDashboardDto.FromEntity(result.Value));
    }

    [HttpPost("next")]
    public async Task<IActionResult> Next(int veilingId)
    {
        var result = await _service.ActivateNextProductAsync(veilingId);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(VMCurrentProductDto.FromEntity(result.Value));
    }

    [HttpPost("close-current")]
    public async Task<IActionResult> Close(int veilingId)
    {
        var result = await _service.CloseCurrentProductAsync(veilingId);
        if (!result.Success) return BadRequest(result.Error);

        return Ok(new { closed = true });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(int veilingId)
    {
        var result = await _service.GetDashboardStateAsync(veilingId);
        if (!result.Success) return NotFound(result.Error);

        return Ok(VMVeilingDashboardDto.FromEntity(result.Value));
    }
} 