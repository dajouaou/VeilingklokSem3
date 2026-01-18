using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Services
{
    // BackgroundService die op de achtergrond draait en elke seconde checkt
    // of er een actieve veiling is waarvan de prijs omlaag moet.
    public class PrijsMechanismeService : BackgroundService
    {
        // ScopeFactory nodig omdat BackgroundService buiten request-scope draait.
        // Zo kunnen we per tick een nieuwe scope maken met DbContext + broadcasters.
        private readonly IServiceScopeFactory _scopeFactory;

        public PrijsMechanismeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // Dit is de “main loop” van de background service.
        // Blijft draaien totdat de app stopt of stoppingToken wordt gecanceld.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int tickMs = 1000;   // frequentie: elke seconde (1 tick)
            const int stepSeconds = 5; // prijs daalt pas elke 5 seconden
            const decimal defaultDalingPerSeconde = 0.01m; // fallback als er geen daling is ingesteld

            var delay = TimeSpan.FromMilliseconds(tickMs);

            // Loop zolang de service actief is
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Nieuwe scope aanmaken zodat we scoped services veilig kunnen gebruiken
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
                    var broadcast = scope.ServiceProvider.GetRequiredService<IVeilingBroadcastService>();

                    // Haal alle veilingen op die momenteel gestart zijn, inclusief producten + aanmelding + aanvoerder
                    var actieveVeilingen = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gestart)
                        .Include(v => v.Producten)
                            .ThenInclude(p => p.Aanmelding)
                                .ThenInclude(a => a.Aanvoerder)
                        .ToListAsync(stoppingToken);

                    // Tijdstip gebruiken voor prijslogica (UTC is stabiel voor server-side tijd)
                    var nowUtc = DateTime.UtcNow;

                    // Loop door alle actieve veilingen heen
                    foreach (var v in actieveVeilingen)
                    {
                        // Huidige product zoeken aan de hand van HuidigProductId
                        var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
                        if (hp == null) continue;

                        // Alleen doorwerken als product actief is en nog niet verkocht/doorgedraaid
                        if (!hp.IsActief || hp.IsVerkocht || hp.IsDoorgedraaid) continue;

                        // Veiligheidsnet: basiswaarden invullen als ze niet goed staan (0 of negatief)
                        // Zo voorkom je fouten bij “onvolledige” data.
                        if (hp.MinimumPrijs <= 0) hp.MinimumPrijs = hp.Aanmelding?.MinimumPrijs ?? 0m;
                        if (hp.MaximumPrijs <= 0) hp.MaximumPrijs = hp.MinimumPrijs + 5m;
                        if (hp.DalingPerSeconde <= 0) hp.DalingPerSeconde = defaultDalingPerSeconde;
                        if (hp.ResterendeHoeveelheid <= 0) hp.ResterendeHoeveelheid = hp.Aanmelding?.Hoeveelheid ?? 0;
                        if (hp.HuidigePrijs <= 0) hp.HuidigePrijs = hp.MaximumPrijs;

                        // Eerste keer: we zetten alleen LaatstePrijsUpdateUtc zodat we een startpunt hebben.
                        // Anders zou hij meteen kunnen dalen bij de eerste tick.
                        if (hp.LaatstePrijsUpdateUtc == null)
                        {
                            hp.LaatstePrijsUpdateUtc = nowUtc;
                            await db.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        // Alleen prijs verlagen als er minimaal stepSeconds tijd is verstreken
                        var elapsed = (nowUtc - hp.LaatstePrijsUpdateUtc.Value).TotalSeconds;
                        if (elapsed < stepSeconds) continue;

                        // Laatste update tijd bijwerken en nieuwe prijs berekenen
                        hp.LaatstePrijsUpdateUtc = nowUtc;
                        var nieuwePrijs = hp.HuidigePrijs - (hp.DalingPerSeconde * stepSeconds);

                        // Als we onder of op de minimumprijs uitkomen: product draait door
                        if (nieuwePrijs <= hp.MinimumPrijs)
                        {
                            // Zet prijs op minimum en markeer als doorgedraaid/niet-actief
                            hp.HuidigePrijs = hp.MinimumPrijs;
                            hp.IsDoorgedraaid = true;
                            hp.IsActief = false;

                            // Zoek volgende product in wachtrij (niet verkocht, niet doorgedraaid, niet actief)
                            var volgende = v.Producten
                                .Where(p => !p.IsVerkocht && !p.IsDoorgedraaid && !p.IsActief)
                                .OrderBy(p => p.Volgorde)
                                .FirstOrDefault();

                            // Als er een volgend product is: zet die actief en initieer startwaarden
                            if (volgende != null)
                            {
                                volgende.IsActief = true;
                                volgende.MinimumPrijs = volgende.Aanmelding?.MinimumPrijs ?? 0m;
                                volgende.MaximumPrijs = volgende.MinimumPrijs + 5m;
                                volgende.DalingPerSeconde = defaultDalingPerSeconde;
                                volgende.ResterendeHoeveelheid = volgende.Aanmelding?.Hoeveelheid ?? 0;
                                volgende.HuidigePrijs = volgende.MaximumPrijs;
                                volgende.LaatstePrijsUpdateUtc = nowUtc;

                                // Update huidig product id in de veiling
                                v.HuidigProductId = volgende.Id;
                            }
                            else
                            {
                                // Geen producten meer: veiling afsluiten
                                v.Status = VeilingStatus.Afgesloten;
                                v.EindTijd = nowUtc;
                                v.HuidigProductId = null;
                            }

                            // Database opslaan voor statuswijzigingen
                            await db.SaveChangesAsync(stoppingToken);

                            // Audit event sturen (bijv. zichtbaar in dashboard)
                            await broadcast.StuurAuditEvent(v.Id, new AuditEventDto
                            {
                                Gebeurtenis = $"Doordraai: {hp.Aanmelding?.Soort} bereikte minimumprijs.",
                                Tijdstip = nowUtc
                            });

                            continue;
                        }

                        // Normale prijsdaling: prijs bijwerken en opslaan
                        hp.HuidigePrijs = nieuwePrijs;
                        await db.SaveChangesAsync(stoppingToken);

                        // Nieuwe productstatus broadcasten naar clients (SignalR)
                        await broadcast.StuurHuidigProduct(v.Id, new HuidigProductDto
                        {
                            VeilingProductId = hp.Id,
                            Soort = hp.Aanmelding!.Soort,
                            FotoUrl = hp.Aanmelding!.FotoUrl,
                            MaximumPrijs = hp.MaximumPrijs,
                            MinimumPrijs = hp.MinimumPrijs,
                            HuidigePrijs = hp.HuidigePrijs,
                            DalingPerSeconde = hp.DalingPerSeconde,
                            ResterendeHoeveelheid = hp.ResterendeHoeveelheid,
                            IsActief = hp.IsActief,
                            IsVerkocht = hp.IsVerkocht,
                            IsDoorgedraaid = hp.IsDoorgedraaid,
                            AanvoerderId = hp.Aanmelding!.AanvoerderId,
                            AanvoerderNaam = hp.Aanmelding!.Aanvoerder?.Naam ?? ""
                        });
                    }
                }
                catch (TaskCanceledException)
                {
                    // Normale situatie bij stoppen van de applicatie/service: niks doen
                }
                catch (Exception ex)
                {
                    // Vangt onverwachte fouten zodat de background service niet crasht
                    Console.WriteLine("PrijsMechanismeService error: " + ex);
                }

                // Wacht tot volgende tick (en stop direct als stoppingToken gecanceld wordt)
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
