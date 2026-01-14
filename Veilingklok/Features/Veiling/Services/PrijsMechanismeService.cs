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
        private const int TickMs = 1000;
        private const int StepSeconds = 5;
        private const decimal DefaultDalingPerSeconde = 0.01m;

        private readonly IServiceScopeFactory _scopeFactory;

        // Constructor: bewaart de scope factory zodat we per tick een scope kunnen maken
        // en netjes scoped services (DbContext, broadcaster) kunnen ophalen.
        public PrijsMechanismeService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // Tijd-provider: hieromheen kan je in tests een vaste tijd teruggeven.
        internal virtual DateTime UtcNow() => DateTime.UtcNow;

        // Background loop: draait elke seconde en roept 1 "tick" aan.
        // Als de app stopt, wordt de CancellationToken gecanceld en stopt deze loop netjes.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromMilliseconds(TickMs);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunSingleTickAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PrijsMechanismeService error: " + ex);
                }

                await Task.Delay(delay, stoppingToken);
            }
        }

        // 1 tick: haalt alle actieve veilingen op en verwerkt ze stuk voor stuk.
        // Dit is expres losgemaakt zodat je dit makkelijk kan unit-testen.
        internal async Task RunSingleTickAsync(CancellationToken stoppingToken)
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

            var nowUtc = UtcNow();

            foreach (var v in actieveVeilingen)
            {
                await ProcessVeilingAsync(db, broadcast, v, nowUtc, stoppingToken);
            }
        }

        // Verwerkt 1 veiling: pakt het huidige product, checkt of het nog "loopt",
        // zet defaults als nodig, en doet daarna óf een normale prijsdaling óf een doordraai.
        private async Task ProcessVeilingAsync(
            MyContext db,
            IVeilingBroadcastService broadcast,
            Core.Entities.Veiling v,
            DateTime nowUtc,
            CancellationToken stoppingToken)
        {
            var hp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
            if (hp == null) return;

            if (!hp.IsActief || hp.IsVerkocht || hp.IsDoorgedraaid) return;

            ApplyDefaultsIfNeeded(hp);

            if (hp.HuidigePrijs <= 0) hp.HuidigePrijs = hp.MaximumPrijs;

            if (hp.LaatstePrijsUpdateUtc == null)
            {
                hp.LaatstePrijsUpdateUtc = nowUtc;
                await db.SaveChangesAsync(stoppingToken);
                return;
            }

            var elapsed = (nowUtc - hp.LaatstePrijsUpdateUtc.Value).TotalSeconds;
            if (elapsed < StepSeconds) return;

            hp.LaatstePrijsUpdateUtc = nowUtc;

            var dalingPerStap = hp.DalingPerSeconde * StepSeconds;
            var nieuwePrijs = hp.HuidigePrijs - dalingPerStap;

            if (nieuwePrijs <= hp.MinimumPrijs)
            {
                await HandleMinimumReachedAsync(db, broadcast, v, hp, nowUtc, stoppingToken);
                return;
            }

            hp.HuidigePrijs = nieuwePrijs;
            await db.SaveChangesAsync(stoppingToken);

            await broadcast.StuurHuidigProduct(v.Id, ToHuidigProductDto(hp));
        }

        // Zet veilige defaults op een product als die nog niet goed gevuld zijn.
        // Belangrijk: dit verandert niet zomaar de huidige prijs, alleen ontbrekende instellingen.
        private static void ApplyDefaultsIfNeeded(Core.Entities.VeilingProduct p)
        {
            if (p.MinimumPrijs <= 0) p.MinimumPrijs = p.Aanmelding?.MinimumPrijs ?? 0m;
            if (p.MaximumPrijs <= 0) p.MaximumPrijs = p.MinimumPrijs + 5m;
            if (p.DalingPerSeconde <= 0) p.DalingPerSeconde = DefaultDalingPerSeconde;
            if (p.ResterendeHoeveelheid <= 0) p.ResterendeHoeveelheid = p.Aanmelding?.Hoeveelheid ?? 0;
        }

        // Kiest het volgende product uit de lijst dat nog niet verkocht/doorgedraaid is
        // en nu niet actief is, met de laagste volgorde (dus "volgende op de klok").
        private static Core.Entities.VeilingProduct? FindNextProduct(Core.Entities.Veiling v)
        {
            return v.Producten
                .Where(p => !p.IsVerkocht && !p.IsDoorgedraaid && !p.IsActief)
                .OrderBy(p => p.Volgorde)
                .FirstOrDefault();
        }

        // Wordt aangeroepen zodra de minimumprijs is bereikt.
        // Zet huidig product op doordraai, activeert eventueel het volgende product,
        // slaat alles op en pusht updates via SignalR (audit, wachtrij, huidig product).
        private async Task HandleMinimumReachedAsync(
            MyContext db,
            IVeilingBroadcastService broadcast,
            Core.Entities.Veiling v,
            Core.Entities.VeilingProduct hp,
            DateTime nowUtc,
            CancellationToken stoppingToken)
        {
            hp.HuidigePrijs = hp.MinimumPrijs;
            hp.IsDoorgedraaid = true;
            hp.IsActief = false;

            var volgende = FindNextProduct(v);

            if (volgende != null)
            {
                volgende.IsActief = true;

                ApplyDefaultsIfNeeded(volgende);

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

            await broadcast.StuurWachtrij(v.Id, BuildWachtrij(v));

            var nieuwHp = v.Producten.SingleOrDefault(p => p.Id == v.HuidigProductId);
            if (nieuwHp?.Aanmelding != null)
            {
                await broadcast.StuurHuidigProduct(v.Id, ToHuidigProductDto(nieuwHp));
            }
        }

        // Bouwt de wachtrij DTO-list: alle producten die nog wachten (niet actief, niet verkocht, niet doorgedraaid)
        // in volgorde, zodat de frontend precies weet wat er nog aankomt.
        private static List<WachtrijItemDto> BuildWachtrij(Core.Entities.Veiling v)
        {
            return v.Producten
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
        }

        // Mapt een VeilingProduct entity naar de DTO die de frontend nodig heeft voor het "huidig product" scherm.
        private static HuidigProductDto ToHuidigProductDto(Core.Entities.VeilingProduct p)
        {
            return new HuidigProductDto
            {
                VeilingProductId = p.Id,
                Soort = p.Aanmelding!.Soort,
                FotoUrl = p.Aanmelding!.FotoUrl,
                MaximumPrijs = p.MaximumPrijs,
                MinimumPrijs = p.MinimumPrijs,
                HuidigePrijs = p.HuidigePrijs,
                DalingPerSeconde = p.DalingPerSeconde,
                ResterendeHoeveelheid = p.ResterendeHoeveelheid,
                IsActief = p.IsActief,
                IsVerkocht = p.IsVerkocht,
                IsDoorgedraaid = p.IsDoorgedraaid,
                AanvoerderId = p.Aanmelding!.AanvoerderId,
                AanvoerderNaam = p.Aanmelding!.Aanvoerder?.Naam ?? ""
            };
        }
    }
}
