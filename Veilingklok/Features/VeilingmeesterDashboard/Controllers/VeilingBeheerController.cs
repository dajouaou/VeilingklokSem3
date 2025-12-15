using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

[ApiController]
[Authorize(Roles = "Veilingmeester")]
[Route("api/veilingmeester/veilingen")]
public class VeilingBeheerController : ControllerBase
{
    private readonly IVeilingService _service;
    private readonly IVeilingBroadcastService _broadcast;
    private readonly MyContext _db;

    public VeilingBeheerController(
        IVeilingService service,
        IVeilingBroadcastService broadcast,
        MyContext db)
    {
        _service = service;
        _broadcast = broadcast;
        _db = db;
    }

    [HttpGet("actief")]
    public async Task<ActionResult<VeilingOverzichtDto?>> GetActieve()
        => Ok(await _service.GetActieveVeilingAsync());

    [HttpPost("{id}/start")]
    public async Task<ActionResult<VeilingOverzichtDto>> Start(int id)
    {
        var entity = await _db.Veilingen.FindAsync(id);
        if (entity == null)
            return BadRequest("Veiling bestaat niet.");

        var geplandeStart = entity.Datum.Date + entity.StartTijd;
        if (DateTime.Now < geplandeStart)
            return BadRequest($"Veiling kan pas starten op {geplandeStart:yyyy-MM-dd HH:mm}");

        var overzicht = await _service.StartGeplandeVeilingAsync(id);

        if (overzicht.HuidigProduct != null)
        {
            await _broadcast.StuurHuidigProduct(id, overzicht.HuidigProduct);
            await _broadcast.StuurWachtrij(id, overzicht.Wachtrij);
        }

        return Ok(overzicht);
    }

    [HttpPost("{id}/pause")]
    public async Task<IActionResult> Pause(int id)
    {
        await _service.PauseAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/resume")]
    public async Task<IActionResult> Resume(int id)
    {
        await _service.ResumeAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/stop")]
    public async Task<IActionResult> Stop(int id)
    {
        await _service.StopAsync(id);
        return NoContent();
    }
}
