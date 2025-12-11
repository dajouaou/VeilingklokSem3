// src/Features/Veiling/IVeilingService.cs
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling;

public interface IVeilingService
{
    Task<Result<PublicVeilingDto>> LoadPublicAsync(int veilingId);

    Task<Result<BidResultDto>> PlaceBidAsync(int veilingId, int koperId);
}