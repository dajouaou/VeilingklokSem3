// Veilingklok/Features/VM/VMController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Features.VM.Services;

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

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboard(int veilingId)
    {
        var result = await _service.GetDashboardAsync(veilingId);
        if (!result.Success)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Start(int veilingId)
    {
        var result = await _service.StartVeilingAsync(veilingId);
        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("next")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Next(int veilingId)
    {
        var result = await _service.ActivateNextProductAsync(veilingId);
        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("close-current")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Close(int veilingId)
    {
        var result = await _service.CloseCurrentProductAsync(veilingId);
        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
