using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Auth.Dtos
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public UserRole Rol { get; set; } // Koper of Aanvoerder
    }
}
