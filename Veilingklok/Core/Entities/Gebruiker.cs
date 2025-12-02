using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Gebruiker
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? Naam { get; set; }

    public UserRole Role { get; set; } = UserRole.Koper;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Aanvoerder? Aanvoerder { get; set; }
    public Koper? Koper { get; set; }
    public Veilingmeester? Veilingmeester { get; set; }
}