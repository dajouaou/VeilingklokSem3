using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Authorize(Roles = "Koper")]
    [Route("api/bod")]
    // Controller voor het plaatsen van biedingen door kopers
    public class BodController : ControllerBase
    {
        private readonly IVeilingService _service;
        private readonly IVeilingBroadcastService _broadcast;
        private readonly MyContext _db;

        // Injecteert veilingservice, broadcaster en database
        public BodController(IVeilingService service, IVeilingBroadcastService broadcast, MyContext db)
        {
            _service = service;
            _broadcast = broadcast;
            _db = db;
        }

        [HttpPost("{veilingId:int}")]
        // Endpoint om een bod te plaatsen op een veiling
        public async Task<IActionResult> Plaats(int veilingId, [FromBody] BodPlaatsenDto dto)
        {
            // Haalt koper-id uit de token
            var koperGebruikerId = await ResolveKoperGebruikerIdAsync();
            if (koperGebruikerId <= 0)
                return Unauthorized("Kon koper-id niet bepalen uit token.");

            // Plaatst het bod via de service
            var bod = await _service.PlaatsBodAsync(veilingId, dto, koperGebruikerId);
            var overzicht = await _service.GetDetailsAsync(veilingId);

            // Stuurt bod realtime naar clients
            await _broadcast.StuurBod(veilingId, bod);

            // Stuurt huidig product als dat bestaat
            if (overzicht.HuidigProduct != null)
                await _broadcast.StuurHuidigProduct(veilingId, overzicht.HuidigProduct);

            // Stuurt bijgewerkte wachtrij
            await _broadcast.StuurWachtrij(veilingId, overzicht.Wachtrij);

            // Stuurt audit-event voor logging/tijdlijn
            await _broadcast.StuurAuditEvent(veilingId, new AuditEventDto
            {
                Gebeurtenis = $"Koop: VP#{dto.VeilingProductId} voor {bod.Prijs:0.00} EUR (aantal {(dto.Aantal <= 0 ? "alles" : dto.Aantal.ToString())}).",
                Tijdstip = DateTime.UtcNow
            });

            // Geeft het bod terug aan de client
            return Ok(bod);
        }


        // Haalt koper-gebruikerId uit token en checkt of die echt een koper is
        private async Task<int> ResolveKoperGebruikerIdAsync()
        {
            var userIdStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("userId") ??
                User.FindFirstValue("id");

            if (!int.TryParse(userIdStr, out var gebruikerId))
                return 0;

            // Checkt of deze gebruiker een koper-profiel heeft
            var exists = await _db.Kopers.AnyAsync(k => k.GebruikerId == gebruikerId);
            return exists ? gebruikerId : 0;
        }

    }
}
