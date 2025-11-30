using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Gebruiker
{
    public int Id { get; set; }                 // PK
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public UserRole Role { get; set; } = UserRole.Koper;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // 1-op-0/1 profielen
    public Aanvoerder? Aanvoerder { get; set; }
    public  Koper? Koper { get; set; }
}