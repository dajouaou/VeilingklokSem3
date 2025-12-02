using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Controllers;

[ApiController]
[Route("api/veilingen/{veilingId:int}/audit")]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditService _audit;

    public AuditController(IAuditService audit)
    {
        _audit = audit;
    }

    // ---------------------------------------------------------
    // GET audit for specific veiling
    // ---------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = $"{nameof(UserRole.Veilingmeester)},{nameof(UserRole.Admin)}")]
    [ProducesResponseType(typeof(List<AuditEntryDto>), 200)]
    public async Task<IActionResult> Get(int veilingId)
        => Ok((await _audit.GetByVeilingAsync(veilingId)).Value);

    // ---------------------------------------------------------
    // GET recent audit (admin only)
    // ---------------------------------------------------------
    [HttpGet("~/api/audit/recent")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(List<AuditEntryDto>), 200)]
    public async Task<IActionResult> GetRecent()
        => Ok((await _audit.GetRecentAsync(50)).Value);
}