using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Auth.Dtos;

public sealed class GebruikerDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Naam { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}