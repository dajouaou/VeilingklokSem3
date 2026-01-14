using Veilingklok.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Veilingklok.Features.Auth.Services
{
    // Service die JWT tokens maakt voor ingelogde gebruikers
    public class JwtService
    {
        private readonly string _key;

        // Leest de JWT secret key uit de configuratie
        public JwtService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(_key))
                throw new Exception("JWT Key is niet ingesteld in appsettings.json!");
        }

        // Maakt een JWT token op basis van de gebruiker
        public string GenerateToken(Gebruiker gebruiker)
        {
            // Bouwt de claims die in het token komen
            var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, gebruiker.Id.ToString()),
    new Claim(ClaimTypes.Email, gebruiker.Email),
    new Claim(ClaimTypes.Role, gebruiker.Rol.ToString()),
    new Claim(ClaimTypes.GivenName, gebruiker.Voornaam),
    new Claim(ClaimTypes.Surname, gebruiker.Achternaam),
    new Claim(ClaimTypes.Name, $"{gebruiker.Voornaam} {gebruiker.Achternaam}")

        };

            // Maakt een security key van de geheime sleutel
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Bouwt het JWT token met claims, verloopdatum en handtekening
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            // Zet het token om naar een string voor de client
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
