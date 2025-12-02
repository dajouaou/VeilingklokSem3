namespace Veilingklok.Features.Auth.Dtos;

public sealed class LoginDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Wachtwoord { get; set; } = string.Empty;
}