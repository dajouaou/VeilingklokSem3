using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Services
{
    public class PrijsMechanismeService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PrijsMechanismeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int tickMs = 1000;      // loop elke seconde
            const int stepSeconds = 5;    // prijs-update elke 5 sec

            // tempo (default) als product geen daling heeft
            const decimal defaultDalingPerSeconde = 0.01m;

            var delay = TimeSpan.FromMilliseconds(tickMs);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
                    var broadcast = scope.ServiceProvider.GetRequiredService<IVeilingBroadcastService>();

                    var actieveVeilingen = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gestart)
                        .Include(v => v.Producten)
                            .ThenInclude(p => p.Aanmelding)
                                .ThenInclude(a => a.Aanvoerder)
                        .ToListAsync(stoppingToken);

                    var nowUtc = DateTime.UtcNow;

                    foreach (var v in actieveVeilingen)
                    {
                        var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
                        if (hp == null) continue;

                        // Alleen dalen als product "actief" is en nog niet klaar
                        if (!hp.IsActief || hp.IsVerkocht || hp.IsDoorgedraaid) continue;

                        // -----------------------------
                        // 1) Defaults (altijd veilig zetten)
                        //    -> NIET de huidige prijs resetten
                        // -----------------------------
                        if (hp.MinimumPrijs <= 0) hp.MinimumPrijs = hp.Aanmelding?.MinimumPrijs ?? 0m;
                        if (hp.MaximumPrijs <= 0) hp.MaximumPrijs = hp.MinimumPrijs + 5m;
                        if (hp.DalingPerSeconde <= 0) hp.DalingPerSeconde = defaultDalingPerSeconde;
                        if (hp.ResterendeHoeveelheid <= 0) hp.ResterendeHoeveelheid = hp.Aanmelding?.Hoeveelheid ?? 0;

                        // Als HuidigePrijs nog niet gezet is, start bovenaan (1x)
                        if (hp.HuidigePrijs <= 0) hp.HuidigePrijs = hp.MaximumPrijs;

                        // -----------------------------
                        // 2) Init timer (1x) + skip daling deze tick
                        //    -> voorkomt "direct dalen" en voorkomt "reset bug"
                        // -----------------------------
                        if (hp.LaatstePrijsUpdateUtc == null)
                        {
                            hp.LaatstePrijsUpdateUtc = nowUtc;
                            await db.SaveChangesAsync(stoppingToken);
                            continue; // volgende tick pas dalen
                        }

                        // -----------------------------
                        // 3) Alleen elke stepSeconds updaten
                        // -----------------------------
                        var elapsed = (nowUtc - hp.LaatstePrijsUpdateUtc.Value).TotalSeconds;
                        if (elapsed < stepSeconds) continue;

                        hp.LaatstePrijsUpdateUtc = nowUtc;

                        var dalingPerStap = hp.DalingPerSeconde * stepSeconds;
                        var nieuwePrijs = hp.HuidigePrijs - dalingPerStap;

                        // -----------------------------
                        // 4) Minimum bereikt => doordraai
                        // -----------------------------
                        if (nieuwePrijs <= hp.MinimumPrijs)
                        {
                            hp.HuidigePrijs = hp.MinimumPrijs;
                            hp.IsDoorgedraaid = true;
                            hp.IsActief = false;

                            var volgende = v.Producten
                                .Where(p => !p.IsVerkocht && !p.IsDoorgedraaid && !p.IsActief)
                                .OrderBy(p => p.Volgorde)
                                .FirstOrDefault();

                            if (volgende != null)
                            {
                                volgende.IsActief = true;

                                // defaults volgende product
                                if (volgende.MinimumPrijs <= 0) volgende.MinimumPrijs = volgende.Aanmelding?.MinimumPrijs ?? 0m;
                                if (volgende.MaximumPrijs <= 0) volgende.MaximumPrijs = volgende.MinimumPrijs + 5m;
                                if (volgende.DalingPerSeconde <= 0) volgende.DalingPerSeconde = defaultDalingPerSeconde;
                                if (volgende.ResterendeHoeveelheid <= 0) volgende.ResterendeHoeveelheid = volgende.Aanmelding?.Hoeveelheid ?? 0;

                                // start bovenaan + timer init
                                volgende.HuidigePrijs = volgende.MaximumPrijs;
                                volgende.LaatstePrijsUpdateUtc = nowUtc;

                                v.HuidigProductId = volgende.Id;
                            }
                            else
                            {
                                v.Status = VeilingStatus.Afgesloten;
                                v.EindTijd = nowUtc;
                                v.HuidigProductId = null;
                            }

                            await db.SaveChangesAsync(stoppingToken);

                            // Audit
                            await broadcast.StuurAuditEvent(v.Id, new AuditEventDto
                            {
                                Gebeurtenis = $"Doordraai: {hp.Aanmelding?.Soort} bereikte minimumprijs.",
                                Tijdstip = nowUtc
                            });

                            // Wachtrij
                            var wachtrij = v.Producten
                                .Where(p => !p.IsActief && !p.IsVerkocht && !p.IsDoorgedraaid)
                                .OrderBy(p => p.Volgorde)
                                .Select(p => new WachtrijItemDto
                                {
                                    VeilingProductId = p.Id,
                                    Volgorde = p.Volgorde,
                                    Soort = p.Aanmelding!.Soort,
                                    FotoUrl = p.Aanmelding!.FotoUrl,
                                    MaximumPrijs = p.MaximumPrijs,
                                    MinimumPrijs = p.MinimumPrijs,
                                    ResterendeHoeveelheid = p.ResterendeHoeveelheid,
                                    AanvoerderId = p.Aanmelding!.AanvoerderId,
                                    AanvoerderNaam = p.Aanmelding!.Aanvoerder?.Naam ?? ""
                                })
                                .ToList();

                            await broadcast.StuurWachtrij(v.Id, wachtrij);

                            // Nieuw huidig product
                            var nieuwHp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
                            if (nieuwHp?.Aanmelding != null)
                            {
                                await broadcast.StuurHuidigProduct(v.Id, new HuidigProductDto
                                {
                                    VeilingProductId = nieuwHp.Id,
                                    Soort = nieuwHp.Aanmelding.Soort,
                                    FotoUrl = nieuwHp.Aanmelding.FotoUrl,
                                    MaximumPrijs = nieuwHp.MaximumPrijs,
                                    MinimumPrijs = nieuwHp.MinimumPrijs,
                                    HuidigePrijs = nieuwHp.HuidigePrijs,
                                    DalingPerSeconde = nieuwHp.DalingPerSeconde,
                                    ResterendeHoeveelheid = nieuwHp.ResterendeHoeveelheid,
                                    IsActief = nieuwHp.IsActief,
                                    IsVerkocht = nieuwHp.IsVerkocht,
                                    IsDoorgedraaid = nieuwHp.IsDoorgedraaid,
                                    AanvoerderId = nieuwHp.Aanmelding.AanvoerderId,
                                    AanvoerderNaam = nieuwHp.Aanmelding.Aanvoerder?.Naam ?? ""
                                });
                            }

                            continue;
                        }

                        // -----------------------------
                        // 5) Normale daling
                        // -----------------------------
                        hp.HuidigePrijs = nieuwePrijs;

                        await db.SaveChangesAsync(stoppingToken);

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
                    // shutdown
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PrijsMechanismeService error: " + ex);
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
