using System.Security.Claims;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;

public sealed class UserContext : IUserContext
{
    public int UserId { get; }
    public string Username { get; }
    public UserRole Role { get; }

    public UserContext(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;

        if (user == null)
        {
            UserId = 0;
            Username = "";
            Role = UserRole.Koper;
            return;
        }

        UserId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Username = user.Identity?.Name ?? "";
        Role = Enum.Parse<UserRole>(user.FindFirstValue(ClaimTypes.Role)!);
    }
}