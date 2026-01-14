using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services
{
    // Background service die verlopen geplande veilingen automatisch afsluit
    public class VeilingCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // Injecteert de scope factory om DbContext per run op te halen
        public VeilingCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // Achtergrondloop die elke 30 seconden controleert op verlopen veilingen
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Maakt een scope en haalt de database op
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();

                    var now = DateTime.Now;
                    var today = now.Date;

                    // Haalt geplande veilingen op van vandaag en eerder
                    var kandidaten = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gepland && v.Datum <= today)
                        .ToListAsync(stoppingToken);

                    // Bepaalt hoeveel speling na starttijd is toegestaan
                    var grace = TimeSpan.FromMinutes(5);

                    // Filtert veilingen die definitief verlopen zijn
                    var verlopen = kandidaten
                        .Where(v =>
                        {
                            var geplandeStart = v.Datum.Date + v.StartTijd;

                            // Alles van eerdere dagen meteen afsluiten
                            if (v.Datum.Date < today) return true;

                            // Vandaag pas na de grace-periode afsluiten
                            return now > geplandeStart + grace;
                        })
                        .ToList();

                    // Zet verlopen veilingen op afgesloten
                    if (verlopen.Count > 0)
                    {
                        foreach (var v in verlopen)
                            v.Status = VeilingStatus.Afgesloten;

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch
                {
                    // Fouten hier mogen de service niet stoppen
                }

                // Wacht 30 seconden tot de volgende check
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
