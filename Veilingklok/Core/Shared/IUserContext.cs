using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Interfaces;

public interface IUserContext
{
    int UserId { get; }
    string Username { get; }
    UserRole Role { get; }
}