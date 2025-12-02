using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Auth.Dtos;

public sealed class AuthResponseDto
{
    public int GebruikerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
}