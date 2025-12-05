using Veilingklok.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Veilingklok.Features.Auth.Services
{
    public class JwtService
    {
        private readonly string _key;

        public JwtService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(_key))
                throw new Exception("JWT Key is niet ingesteld in appsettings.json!");
        }

        public string GenerateToken(Gebruiker gebruiker)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, gebruiker.Id.ToString()),
        new Claim(ClaimTypes.Email, gebruiker.Email),
        new Claim(ClaimTypes.Role, gebruiker.Role.ToString())  // 
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
