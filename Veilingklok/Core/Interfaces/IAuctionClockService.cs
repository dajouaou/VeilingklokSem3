// Veilingklok/Features/Veiling/Services/IAuctionClockService.cs
using System.Threading;
using System.Threading.Tasks;

namespace Veilingklok.Features.Veiling.Services;

public interface IAuctionClockService
{
    Task TickAsync(CancellationToken ct);
}