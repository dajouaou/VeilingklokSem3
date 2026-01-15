using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Time;

namespace Veilingklok.Features.Veiling.Services
{
    // Background service die verlopen geplande veilingen automatisch afsluit
    public class VeilingCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public VeilingCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();

                    // ✅ Gebruik NL tijd
                    var now = NlTime.Now();
                    var today = now.Date;

                    // geplande veilingen van vandaag en eerder
                    var kandidaten = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gepland && v.Datum <= today)
                        .ToListAsync(stoppingToken);

                    var grace = TimeSpan.FromMinutes(5);

                    var verlopen = kandidaten
                        .Where(v =>
                        {
                            var geplandeStart = v.Datum.Date + v.StartTijd;

                            // alles van eerdere dagen meteen afsluiten
                            if (v.Datum.Date < today) return true;

                            // vandaag pas na grace afsluiten
                            return now > geplandeStart + grace;
                        })
                        .ToList();

                    if (verlopen.Count > 0)
                    {
                        foreach (var v in verlopen)
                            v.Status = VeilingStatus.Afgesloten;

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch
                {
                    // errors mogen service niet stoppen
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
