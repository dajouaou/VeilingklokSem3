// Veilingklok/Features/VM/VMController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Features.VM.Services;

namespace Veilingklok.Features.VM;

[Authorize(Roles = "Veilingmeester")]
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
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetDashboard(int veilingId)
    {
        var result = await _service.GetDashboardAsync(veilingId);
        return ToActionResult(result);
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Start(int veilingId)
    {
        var result = await _service.StartVeilingAsync(veilingId);
        return ToActionResult(result);
    }

    [HttpPost("next")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Next(int veilingId)
    {
        var result = await _service.ActivateNextProductAsync(veilingId);
        return ToActionResult(result);
    }

    [HttpPost("close-current")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Close(int veilingId)
    {
        var result = await _service.CloseCurrentProductAsync(veilingId);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult(Result<VMVeilingDashboardDto> result)
    {
        if (result.Success)
            return Ok(result.Value);

        var error = result.Error ?? "Onbekende fout.";

        if (error.Contains("niet gevonden", StringComparison.OrdinalIgnoreCase))
            return NotFound(error);

        if (error.Contains("al gestart", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Geen actief product", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Geen volgende producten", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("Geen producten gekoppeld", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status409Conflict, error);
        }

        return BadRequest(error);
    }
}
