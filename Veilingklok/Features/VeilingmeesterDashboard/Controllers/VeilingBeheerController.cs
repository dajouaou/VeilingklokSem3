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
// Controller voor veilingmeester acties (start/pause/resume/stop + archief)
public class VeilingBeheerController : ControllerBase
{
    private readonly IVeilingService _service;
    private readonly IVeilingBroadcastService _broadcast;
    private readonly MyContext _db;

    // Injecteert service, broadcaster en database
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
    // Haalt de huidige actieve veiling op
    public async Task<ActionResult<VeilingOverzichtDto?>> GetActieve()
        => Ok(await _service.GetActieveVeilingAsync());

    [HttpPost("{id}/start")]
    // Start een geplande veiling en pusht meteen updates naar clients
    public async Task<ActionResult<VeilingOverzichtDto>> Start(int id)
    {
        // Checkt of de veiling bestaat
        var entity = await _db.Veilingen.FindAsync(id);
        if (entity == null)
            return BadRequest("Veiling bestaat niet.");

        // Blokkeert starten als de geplande starttijd nog niet bereikt is
        var geplandeStart = entity.Datum.Date + entity.StartTijd;
        if (DateTime.Now < geplandeStart)
            return BadRequest($"Veiling kan pas starten op {geplandeStart:yyyy-MM-dd HH:mm}");

        // Start de veiling via de service
        var overzicht = await _service.StartGeplandeVeilingAsync(id);

        // Stuurt huidig product en wachtrij realtime door
        if (overzicht.HuidigProduct != null)
        {
            await _broadcast.StuurHuidigProduct(id, overzicht.HuidigProduct);
            await _broadcast.StuurWachtrij(id, overzicht.Wachtrij);
        }

        return Ok(overzicht);
    }

    [HttpPost("{id}/pause")]
    // Pauzeert een veiling en pusht de nieuwe status naar clients
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
    // Hervat een veiling en pusht de nieuwe status naar clients
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
    // Stopt en sluit een veiling af
    public async Task<IActionResult> Stop(int id)
    {
        await _service.StopAsync(id);
        return NoContent();
    }

    [HttpGet("archief")]
    // Haalt het archief van afgesloten veilingen op inclusief transacties
    public async Task<ActionResult<List<VeilingArchiefDto>>> GetArchief()
    {
        // Laadt afgesloten veilingen met producten, aanmeldingen en transacties
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

        // Zet de entities om naar archief DTO’s
        var result = veilingen.Select(v =>
        {
            // Bepaalt eindtijd van de veiling voor het archief
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

            // Bouwt het archief-item inclusief transactie lijst
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
