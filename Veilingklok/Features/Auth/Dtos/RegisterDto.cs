using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Auth.Dtos;

public sealed class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Wachtwoord { get; set; } = string.Empty;
    public string? Naam { get; set; }
    public UserRole Role { get; set; } = UserRole.Koper;
}