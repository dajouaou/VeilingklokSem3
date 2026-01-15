using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;

namespace Veilingklok.Features.Veiling.Services
{
    // Background service die automatisch de prijs van actieve veilingen laat dalen
    public class PrijsMechanismeService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PrijsMechanismeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int tickMs = 1000;   // elke seconde uitvoeren
            const int stepSeconds = 5; // prijs daalt elke 5 seconden
            const decimal defaultDalingPerSeconde = 0.01m;

            var delay = TimeSpan.FromMilliseconds(tickMs);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
                    var broadcast = scope.ServiceProvider.GetRequiredService<IVeilingBroadcastService>();

                    // Alle actieve veilingen ophalen
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

                        // Alleen actief en nog niet afgerond product
                        if (!hp.IsActief || hp.IsVerkocht || hp.IsDoorgedraaid) continue;

                        // Basiswaarden veilig instellen
                        if (hp.MinimumPrijs <= 0) hp.MinimumPrijs = hp.Aanmelding?.MinimumPrijs ?? 0m;
                        if (hp.MaximumPrijs <= 0) hp.MaximumPrijs = hp.MinimumPrijs + 5m;
                        if (hp.DalingPerSeconde <= 0) hp.DalingPerSeconde = defaultDalingPerSeconde;
                        if (hp.ResterendeHoeveelheid <= 0) hp.ResterendeHoeveelheid = hp.Aanmelding?.Hoeveelheid ?? 0;
                        if (hp.HuidigePrijs <= 0) hp.HuidigePrijs = hp.MaximumPrijs;

                        // Eerste tick alleen timer zetten
                        if (hp.LaatstePrijsUpdateUtc == null)
                        {
                            hp.LaatstePrijsUpdateUtc = nowUtc;
                            await db.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        // Alleen elke stepSeconds dalen
                        var elapsed = (nowUtc - hp.LaatstePrijsUpdateUtc.Value).TotalSeconds;
                        if (elapsed < stepSeconds) continue;

                        hp.LaatstePrijsUpdateUtc = nowUtc;
                        var nieuwePrijs = hp.HuidigePrijs - (hp.DalingPerSeconde * stepSeconds);

                        // Minimumprijs bereikt
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
                                volgende.MinimumPrijs = volgende.Aanmelding?.MinimumPrijs ?? 0m;
                                volgende.MaximumPrijs = volgende.MinimumPrijs + 5m;
                                volgende.DalingPerSeconde = defaultDalingPerSeconde;
                                volgende.ResterendeHoeveelheid = volgende.Aanmelding?.Hoeveelheid ?? 0;
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

                            await broadcast.StuurAuditEvent(v.Id, new AuditEventDto
                            {
                                Gebeurtenis = $"Doordraai: {hp.Aanmelding?.Soort} bereikte minimumprijs.",
                                Tijdstip = nowUtc
                            });

                            continue;
                        }

                        // Normale prijsdaling
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
                    // service stopt
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
