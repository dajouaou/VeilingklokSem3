// Veilingklok/Features/Veilingmeester/Controllers/VeilingmeesterVmController.cs
using System.Security.Claims;
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

    private int GetActorId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Geen geldige gebruiker-id in token.");
        return id;
    }

    [HttpGet("actief")]
    [ProducesResponseType(typeof(VMActiveVeilingDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActieveVeiling()
        => this.ToActionResult(await _vm.GetActieveVeilingAsync());

    [HttpGet("{veilingId:int}/dashboard")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetVmDashboard(int veilingId)
        => this.ToActionResult(await _vm.GetDashboardAsync(veilingId));

    [HttpPost("{veilingId:int}/start")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartVeiling(int veilingId)
        => this.ToActionResult(await _vm.StartVeilingAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/pause")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PauseVeiling(int veilingId)
        => this.ToActionResult(await _vm.PauseVeilingAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/resume")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResumeVeiling(int veilingId)
        => this.ToActionResult(await _vm.ResumeVeilingAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/stop")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StopVeiling(int veilingId)
        => this.ToActionResult(await _vm.StopVeilingAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/next")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> NextProduct(int veilingId)
        => this.ToActionResult(await _vm.ActivateNextProductAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/close-current")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CloseCurrent(int veilingId)
        => this.ToActionResult(await _vm.CloseCurrentProductAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/reset")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResetVeiling(int veilingId)
        => this.ToActionResult(await _vm.ResetVeilingAsync(veilingId, GetActorId()));

    [HttpPost("{veilingId:int}/queue/reorder")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReorderQueue(int veilingId, [FromBody] VMReorderQueueRequest request)
        => this.ToActionResult(await _vm.ReorderQueueAsync(veilingId, request, GetActorId()));

    [HttpPost("{veilingId:int}/queue/skip/{veilingProductId:int}")]
    [ProducesResponseType(typeof(VMVeilingDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SkipProduct(int veilingId, int veilingProductId)
        => this.ToActionResult(await _vm.SkipProductAsync(veilingId, veilingProductId, GetActorId()));
}
