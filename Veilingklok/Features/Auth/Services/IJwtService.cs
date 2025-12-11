using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Auth.Services
{
    public interface IJwtService
    {
        string GenerateToken(Gebruiker gebruiker);
    }
}
