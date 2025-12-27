// Veilingklok/Features/Veilingmeester/Controllers/VeilingmeesterVmController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VM.Dtos;
using Veilingklok.Features.VM.Services;

namespace Veilingklok.Features.Veilingmeester.Controllers;

[Authorize(Roles = "Veilingmeester")]
[ApiController]
[Route("api/veilingmeester/veilingen")]
public sealed class VeilingmeesterVmController : ControllerBase
{
    private readonly IVMService _vm;

    public VeilingmeesterVmController(IVMService vm)
    {
        _vm = vm;
    }

    // GET api/veilingmeester/veilingen/{veilingId}/dashboard
    [HttpGet("{veilingId:int}/dashboard")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetVmDashboard(int veilingId)
        => ToActionResult(await _vm.GetDashboardAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/start
    [HttpPost("{veilingId:int}/start")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartVeiling(int veilingId)
        => ToActionResult(await _vm.StartVeilingAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/pause
    [HttpPost("{veilingId:int}/pause")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PauseVeiling(int veilingId)
        => ToActionResult(await _vm.PauseVeilingAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/resume
    [HttpPost("{veilingId:int}/resume")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResumeVeiling(int veilingId)
        => ToActionResult(await _vm.ResumeVeilingAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/stop
    [HttpPost("{veilingId:int}/stop")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StopVeiling(int veilingId)
        => ToActionResult(await _vm.StopVeilingAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/next
    [HttpPost("{veilingId:int}/next")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> NextProduct(int veilingId)
        => ToActionResult(await _vm.ActivateNextProductAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/close-current
    [HttpPost("{veilingId:int}/close-current")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CloseCurrent(int veilingId)
        => ToActionResult(await _vm.CloseCurrentProductAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/reset
    [HttpPost("{veilingId:int}/reset")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResetVeiling(int veilingId)
        => ToActionResult(await _vm.ResetVeilingAsync(veilingId));

    // POST api/veilingmeester/veilingen/{veilingId}/queue/reorder
    [HttpPost("{veilingId:int}/queue/reorder")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReorderQueue(int veilingId, [FromBody] VMReorderQueueRequest request)
        => ToActionResult(await _vm.ReorderQueueAsync(veilingId, request));

    // POST api/veilingmeester/veilingen/{veilingId}/queue/skip/{veilingProductId}
    [HttpPost("{veilingId:int}/queue/skip/{veilingProductId:int}")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SkipProduct(int veilingId, int veilingProductId)
        => ToActionResult(await _vm.SkipProductAsync(veilingId, veilingProductId));

    // NOTE: dit is nog steeds "string parsing" omdat jouw Result<T> nu alleen Error(string) heeft.
    // Zodra je Result<T> een ErrorCode krijgt, wordt dit een switch op code.
    private IActionResult ToActionResult(Result<VMVeilingDashboardDto> result)
    {
        if (result.Success) return Ok(result.Value);

        var error = result.Error ?? "Onbekende fout.";

        if (error.Contains("niet gevonden", StringComparison.OrdinalIgnoreCase))
            return NotFound(error);

        if (error.Contains("al gestart", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("al beëindigd", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("geen actief product", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("geen volgende producten", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("geen producten gekoppeld", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("is gepauzeerd", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("is niet gepauzeerd", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("kan pas starten", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("kan niet tijdens", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("matchen niet", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(error); // 409
        }

        return BadRequest(error); // 400
    }
}
