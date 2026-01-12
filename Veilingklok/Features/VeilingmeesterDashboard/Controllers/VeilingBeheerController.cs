using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;

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

        var overzicht = await _service.GetDetailsAsync(id);
        if (overzicht.HuidigProduct != null)
        {
            await _broadcast.StuurHuidigProduct(id, overzicht.HuidigProduct);
            await _broadcast.StuurWachtrij(id, overzicht.Wachtrij);
        }

        return NoContent();
    }


    [HttpPost("{id}/resume")]
    public async Task<IActionResult> Resume(int id)
    {
        await _service.ResumeAsync(id);

        var overzicht = await _service.GetDetailsAsync(id);
        if (overzicht.HuidigProduct != null)
        {
            await _broadcast.StuurHuidigProduct(id, overzicht.HuidigProduct);
            await _broadcast.StuurWachtrij(id, overzicht.Wachtrij);
        }

        return NoContent();
    }


    [HttpPost("{id}/stop")]
    public async Task<IActionResult> Stop(int id)
    {
        await _service.StopAsync(id);
        return NoContent();
    }
    [HttpGet("archief")]
    public async Task<ActionResult<List<VeilingArchiefDto>>> GetArchief()
    {
        var veilingen = await _db.Veilingen
            .AsNoTracking()
            .Where(v => v.Status == VeilingStatus.Afgesloten)
            .Include(v => v.Producten)
                .ThenInclude(p => p.Aanmelding)
            .Include(v => v.Producten)
                .ThenInclude(p => p.Transacties)
                    .ThenInclude(t => t.Koper)
            .OrderByDescending(v => v.Datum)
            .ThenByDescending(v => v.StartTijd)
            .ToListAsync();

        var result = veilingen.Select(v =>
        {
            // eindtijd bepalen:
            // 1) als je AfgeslotenOpUtc hebt -> gebruik die
            // 2) anders: laatste transactie
            DateTime? eindUtc = null;

            if (v.AfgeslotenOpUtc != null)
                eindUtc = v.AfgeslotenOpUtc;
            else
            {
                eindUtc = v.Producten?
                    .SelectMany(p => p.Transacties ?? new())
                    .Select(t => (DateTime?)t.Tijdstip)
                    .Max();
            }

            return new VeilingArchiefDto
            {
                Id = v.Id,
                Veildatum = v.Datum.ToString("yyyy-MM-dd"),
                StartTijd = v.StartTijd.ToString(@"hh\:mm"),
                EindTijd = eindUtc.HasValue
    ? eindUtc.Value.ToLocalTime().ToString("HH:mm")
    : "",
                AantalProducten = v.Producten?.Count ?? 0,

                Transacties = (v.Producten ?? new())
                    .SelectMany(p => p.Transacties ?? new())
                    .OrderBy(t => t.Tijdstip)
                    .Select(t => new VeilingTransactieDto
                    {
                        Soort = t.VeilingProduct?.Aanmelding?.Soort ?? "",
                        KoperNaam = t.Koper != null? $"{t.Koper.Voornaam} {t.Koper.Achternaam}": "",
                        Aantal = t.Aantal,
                        Prijs = t.Prijs,
                        Tijdstip = t.Tijdstip
                    })
                    .ToList()
            };
        }).ToList();

        return Ok(result);
    }

}
