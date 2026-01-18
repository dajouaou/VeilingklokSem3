using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Time;

[ApiController]
[Authorize(Roles = "Veilingmeester")] // alleen veilingmeester mag beheren
[Route("api/veilingmeester/veilingen")]
public class VeilingBeheerController : ControllerBase
{
    private readonly IVeilingService _service;            // business logica (start/pause/resume/stop)
    private readonly IVeilingBroadcastService _broadcast; // realtime push naar clients (SignalR)
    private readonly MyContext _db;                       // direct db gebruiken voor archief en bestaan-check

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
        // Geeft actieve veiling terug of null
        => Ok(await _service.GetActieveVeilingAsync());

    [HttpPost("{id}/start")]
    public async Task<ActionResult<VeilingOverzichtDto>> Start(int id)
    {
        // Bestaat de veiling wel?
        var entity = await _db.Veilingen.FindAsync(id);
        if (entity == null)
            return BadRequest(new { message = "Veiling bestaat niet." });

        // Gebruik NL tijd om "te vroeg starten" correct te beoordelen
        var nowNl = NlTime.Now();

        // Geplande start = datum (00:00) + starttijd
        var geplandeStart = entity.Datum.Date + entity.StartTijd;

        // Als je eerder start dan planning => blokkeren met duidelijke foutmelding
        if (nowNl < geplandeStart)
            return BadRequest(new { message = $"Veiling kan pas starten op {geplandeStart:yyyy-MM-dd HH:mm}" });

        // Start via service: service moet de status aanpassen en huidig product bepalen
        var overzicht = await _service.StartGeplandeVeilingAsync(id);

        // Na starten meteen live data pushen zodat clients direct juiste info zien
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
        // Service zet status op pauze
        await _service.PauseAsync(id);

        // Daarna nieuwste details ophalen en pushen (bron van waarheid = database/service)
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
        // Service haalt pauze eraf
        await _service.ResumeAsync(id);

        // Push opnieuw de huidige staat naar clients
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
        // Stop sluit veiling af (status Afgesloten + evt eindtijd opslaan)
        await _service.StopAsync(id);
        return NoContent();
    }

    [HttpGet("archief")]
    public async Task<ActionResult<List<VeilingArchiefDto>>> GetArchief()
    {
        // Laad alle afgesloten veilingen met alle data die je in het archief wilt tonen
        var veilingen = await _db.Veilingen
            .AsNoTracking() // read-only: sneller, geen tracking nodig
            .Where(v => v.Status == VeilingStatus.Afgesloten)
            .Include(v => v.Producten)
                .ThenInclude(p => p.Aanmelding)
            .Include(v => v.Producten)
                .ThenInclude(p => p.Transacties)
                    .ThenInclude(t => t.Koper)
            .OrderByDescending(v => v.Datum)
            .ThenByDescending(v => v.StartTijd)
            .ToListAsync();

        // Mapping van entities naar DTOs voor de frontend (geen database objecten lekken)
        var result = veilingen.Select(v =>
        {
            DateTime? eindUtc = null;

            // Eindmoment bepalen:
            // 1) Als AfgeslotenOpUtc is gezet: gebruik die
            // 2) Anders: neem het laatste transactie tijdstip als "einde"
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
                // eindUtc is UTC => naar lokale tijd voor UI
                EindTijd = eindUtc.HasValue ? eindUtc.Value.ToLocalTime().ToString("HH:mm") : "",
                AantalProducten = v.Producten?.Count ?? 0,

                // Transacties uit alle producten samenvoegen en sorteren op tijd
                Transacties = (v.Producten ?? new())
                    .SelectMany(p => p.Transacties ?? new())
                    .OrderBy(t => t.Tijdstip)
                    .Select(t => new VeilingTransactieDto
                    {
                        Soort = t.VeilingProduct?.Aanmelding?.Soort ?? "",
                        KoperNaam = t.Koper != null ? $"{t.Koper.Voornaam} {t.Koper.Achternaam}" : "",
                        Aantal = t.Aantal,
                        Prijs = t.Prijs,
                        Tijdstip = t.Tijdstip.ToLocalTime()
                    })
                    .ToList()
            };
        }).ToList();

        return Ok(result);
    }
}
