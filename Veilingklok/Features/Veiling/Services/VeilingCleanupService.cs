using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.Time;

namespace Veilingklok.Features.Veiling.Services
{
    // BackgroundService die periodiek (elke 30 seconden) checkt
    // of geplande veilingen verlopen zijn en die dan afsluit.
    public class VeilingCleanupService : BackgroundService
    {
        // ScopeFactory is nodig om binnen een BackgroundService scoped services (zoals DbContext) veilig te gebruiken.
        private readonly IServiceScopeFactory _scopeFactory;

        public VeilingCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // Hoofdloop van de background service: blijft draaien tot de app stopt.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Elke 30 seconden checken is voldoende: het is geen real-time proces
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Nieuwe scope aanmaken voor DbContext (scoped)
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyContext>();

                    // Huidige tijd in NL tijdzone
                    var nowNl = NlTime.Now();
                    var today = nowNl.Date;

                    // Grace period: geplande veilingen krijgen nog 5 minuten marge
                    var grace = TimeSpan.FromMinutes(5);

                    // Kandidaten: veilingen die gepland zijn en vandaag of eerder staan
                    var kandidaten = await db.Veilingen
                        .Where(v => v.Status == VeilingStatus.Gepland && v.Datum <= today)
                        .ToListAsync(stoppingToken);

                    // Filter: bepaal welke veilingen echt verlopen zijn
                    var verlopen = kandidaten
                        .Where(v =>
                        {
                            // Startmoment in NL tijd = datum + starttijd
                            var geplandeStartNl = v.Datum.Date + v.StartTijd;

                            // Als het een eerdere dag is: altijd verlopen
                            if (v.Datum.Date < today) return true;

                            // Als het vandaag is: verlopen zodra we voorbij starttijd + 5 min zijn
                            return nowNl > geplandeStartNl + grace;
                        })
                        .ToList();

                    // Alleen updaten als er echt iets te doen is
                    if (verlopen.Count > 0)
                    {
                        foreach (var v in verlopen)
                        {
                            // Markeer veiling als afgesloten
                            v.Status = VeilingStatus.Afgesloten;

                            // Zet afsluitmoment (UTC) als die nog niet gezet is
                            v.AfgeslotenOpUtc ??= DateTime.UtcNow;
                        }

                        // Wijzigingen opslaan
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch
                {
                    // Fouten negeren zodat de service niet crasht en blijft doorlopen
                    // (Eventueel later vervangen door logging)
                }

                // Wacht 30 seconden tot de volgende check (en stop direct bij cancel)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
