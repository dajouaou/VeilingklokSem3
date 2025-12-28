// Veilingklok/Features/Veiling/Services/IVeilingPublicService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Services;

public interface IVeilingPublicService
{
    Task<Result<List<string>>> GetBeschikbareLeverdagenAsync();
    Task<Result<PublicVeilingDto>> LoadPublicAsync(int veilingId);
}