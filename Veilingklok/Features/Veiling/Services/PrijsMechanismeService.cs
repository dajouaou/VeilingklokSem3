using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using Veilingklok.Core.Enums;

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
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<MyContext>();
                var broadcaster = scope.ServiceProvider.GetRequiredService<IVeilingBroadcastService>();

                var actieveVeilingen = await db.Veilingen
                    .Include(v => v.HuidigProduct)
                        .ThenInclude(p => p.Aanmelding)
                    .Where(v => v.Status == VeilingStatus.Gestart)
                    .ToListAsync(stoppingToken);

                foreach (var v in actieveVeilingen)
                {
                    if (v.HuidigProduct == null)
                        continue;

                    // prijsdaling
                    v.HuidigProduct.HuidigePrijs -= 0.05m;
                    if (v.HuidigProduct.HuidigePrijs < 0)
                        v.HuidigProduct.HuidigePrijs = 0;

                    await db.SaveChangesAsync(stoppingToken);

                    var dto = new HuidigProductDto
                    {
                        VeilingProductId = v.HuidigProduct.Id,
                        Soort = v.HuidigProduct.Aanmelding!.Soort,
                        FotoUrl = v.HuidigProduct.Aanmelding.FotoUrl,
                        Hoeveelheid = v.HuidigProduct.Aanmelding.Hoeveelheid,
                        StartPrijs = v.HuidigProduct.StartPrijs,
                        HuidigePrijs = v.HuidigProduct.HuidigePrijs,
                        IsActief = true,
                        IsVerkocht = v.HuidigProduct.IsVerkocht
                    };

                    await broadcaster.StuurHuidigProduct(v.Id, dto);
                }

                await Task.Delay(300, stoppingToken);
            }
        }
    }
}
