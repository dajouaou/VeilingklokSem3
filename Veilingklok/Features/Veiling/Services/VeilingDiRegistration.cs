// Veilingklok/VeilingDiRegistration.cs
using Microsoft.Extensions.DependencyInjection;
using Veilingklok.Features.Veiling.Services;

namespace Veilingklok;

public static class VeilingDiRegistration
{
    public static IServiceCollection AddVeilingFacade(this IServiceCollection services)
    {
        services.AddScoped<IVeilingPublicService, VeilingPublicService>();
        services.AddScoped<IBiddingService, BiddingService>();
        services.AddScoped<IAuctionClockService, AuctionClockService>();
        services.AddScoped<IVeilingPlanningService, VeilingPlanningService>();
        services.AddHostedService<PrijsMechanismeService>();
        return services;
    }
}