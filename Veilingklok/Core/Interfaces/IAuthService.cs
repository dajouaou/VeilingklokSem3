using Veilingklok.Core.Shared;
using Veilingklok.Features.Auth.Dtos;

namespace Veilingklok.Core.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<GebruikerDto>> GetSelfAsync(int gebruikerId);
}