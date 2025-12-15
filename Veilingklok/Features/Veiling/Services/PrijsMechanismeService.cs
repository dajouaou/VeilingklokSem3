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
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(300, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                // normale shutdown → NIET crashen
            }
        }

    }
}
