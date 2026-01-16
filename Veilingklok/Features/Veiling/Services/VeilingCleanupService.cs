using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Time;

namespace Veilingklok.Features.Veiling.Services
{
    public class VeilingCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public VeilingCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // elke 30 sec checken is prima
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();

                    var nowNl = NlTime.Now();
                    var today = nowNl.Date;

                    var grace = TimeSpan.FromMinutes(5);

                    // Pak geplande veilingen van vandaag en eerder
                    var kandidaten = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gepland && v.Datum <= today)
                        .ToListAsync(stoppingToken);

                    var verlopen = kandidaten
                        .Where(v =>
                        {
                            var geplandeStartNl = v.Datum.Date + v.StartTijd;

                            // Eerdere dagen altijd verlopen
                            if (v.Datum.Date < today) return true;

                            // Vandaag: verlopen als nu > geplande start + 5 min
                            return nowNl > geplandeStartNl + grace;
                        })
                        .ToList();

                    if (verlopen.Count > 0)
                    {
                        foreach (var v in verlopen)
                        {
                            v.Status = VeilingStatus.Afgesloten;

                            // zet timestamp (optioneel maar sterk aan te raden)
                            v.AfgeslotenOpUtc ??= DateTime.UtcNow;
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch
                {
                    // laat de service doorlopen
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
