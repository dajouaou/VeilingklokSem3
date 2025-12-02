using System.Threading.Tasks;
using Veilingklok.Core.Shared;

namespace Veilingklok.Core.Interfaces;

public interface IClockService
{
    Task<Result<bool>> StartClockAsync(int veilingProductId);
}