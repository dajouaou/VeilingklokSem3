using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

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
            const int tickMs = 200;
            var delay = TimeSpan.FromMilliseconds(tickMs);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();
                    var broadcast = scope.ServiceProvider.GetRequiredService<IVeilingBroadcastService>();

                    var actieveVeilingen = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gestart)          // ✅ pauze stop => geen updates
                        .Include(v => v.Producten)
                            .ThenInclude(p => p.Aanmelding)
                        .ToListAsync(stoppingToken);

                    foreach (var v in actieveVeilingen)
                    {
                        // pauze check
                        if (v.Status != VeilingStatus.Gestart) continue;

                        var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);

                        if (hp == null) continue;
                        if (!hp.IsActief || hp.IsVerkocht || hp.IsDoorgedraaid) continue;

                        // defaults als planning nog niet invult
                        if (hp.MinimumPrijs <= 0) hp.MinimumPrijs = hp.Aanmelding?.MinimumPrijs ?? 0;
                        if (hp.MaximumPrijs <= 0) hp.MaximumPrijs = hp.MinimumPrijs + 1m;
                        if (hp.DalingPerSeconde <= 0) hp.DalingPerSeconde = 0.10m;
                        if (hp.ResterendeHoeveelheid <= 0) hp.ResterendeHoeveelheid = hp.Aanmelding?.Hoeveelheid ?? 0;
                        if (hp.HuidigePrijs <= 0) hp.HuidigePrijs = hp.MaximumPrijs;

                        var dalingPerTick = hp.DalingPerSeconde * (tickMs / 1000m);
                        var nieuwePrijs = hp.HuidigePrijs - dalingPerTick;

                        // minimum bereikt => doordraai
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

                                if (volgende.MinimumPrijs <= 0) volgende.MinimumPrijs = volgende.Aanmelding?.MinimumPrijs ?? 0;
                                if (volgende.MaximumPrijs <= 0) volgende.MaximumPrijs = volgende.MinimumPrijs + 1m;
                                if (volgende.DalingPerSeconde <= 0) volgende.DalingPerSeconde = 0.10m;
                                if (volgende.ResterendeHoeveelheid <= 0) volgende.ResterendeHoeveelheid = volgende.Aanmelding?.Hoeveelheid ?? 0;

                                volgende.HuidigePrijs = volgende.MaximumPrijs;
                                v.HuidigProductId = volgende.Id;
                            }
                            else
                            {
                                v.Status = VeilingStatus.Afgesloten;
                                v.EindTijd = DateTime.UtcNow;
                                v.HuidigProductId = null;
                            }

                            await db.SaveChangesAsync(stoppingToken);

                            // broadcast: audit + wachtrij + nieuw huidig product (als die er is)
                            await broadcast.StuurAuditEvent(v.Id, new AuditEventDto
                            {
                                Gebeurtenis = $"Doordraai: {hp.Aanmelding?.Soort} bereikte minimumprijs.",
                                Tijdstip = DateTime.UtcNow
                            });

                            // stuur nieuwe state
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
                                    ResterendeHoeveelheid = p.ResterendeHoeveelheid
                                })
                                .ToList();

                            await broadcast.StuurWachtrij(v.Id, wachtrij);

                            var nieuwHp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
                            if (nieuwHp != null && nieuwHp.Aanmelding != null)
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
                                    IsDoorgedraaid = nieuwHp.IsDoorgedraaid
                                });
                            }

                            continue;
                        }

                        // normale update
                        hp.HuidigePrijs = nieuwePrijs;
                        await db.SaveChangesAsync(stoppingToken);

                        // broadcast enkel huidig product (licht genoeg)
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
                            IsDoorgedraaid = hp.IsDoorgedraaid
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
