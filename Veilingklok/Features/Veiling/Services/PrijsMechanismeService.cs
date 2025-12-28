// Veilingklok/Features/Veiling/Services/PrijsMechanismeService.cs
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Veilingklok.Features.Veiling.Services;

public sealed class PrijsMechanismeService : BackgroundService
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
            await Task.Delay(200, stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAuctionClockService>();

            try
            {
                await service.TickAsync(stoppingToken);
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
        }
    }
}