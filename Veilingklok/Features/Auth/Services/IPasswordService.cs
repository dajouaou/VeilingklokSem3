using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Auth.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPasswordWithSalt);
    }
}
