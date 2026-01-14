using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Auth.Services;
using Xunit;

namespace VeilingklokUnitTest.Auth
{
    public sealed class JwtServiceTests
    {
        [Fact]
        public void Ctor_WithoutJwtKey_ThrowsException()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            var ex = Assert.Throws<Exception>(() => new JwtService(config));

            // minder streng: je message kan "Jwt:Key" of iets anders zijn
            Assert.Contains("Jwt", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GenerateToken_ReturnsValidJwtFormat()
        {
            var service = CreateJwtServiceWithKey("THIS_IS_A_TEST_KEY_MIN_32_CHARS_LONG!!");
            var gebruiker = CreateGebruiker();

            var token = service.GenerateToken(gebruiker);

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Equal(3, token.Split('.').Length);
        }

        [Fact]
        public void GenerateToken_IncludesRoleEmailNameIdClaims()
        {
            var service = CreateJwtServiceWithKey("THIS_IS_A_TEST_KEY_MIN_32_CHARS_LONG!!");
            var gebruiker = CreateGebruiker();

            var tokenString = service.GenerateToken(gebruiker);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);

            string? GetClaim(string type) =>
                jwt.Claims.FirstOrDefault(c => c.Type == type)?.Value;

            Assert.Equal(gebruiker.Id.ToString(), GetClaim(ClaimTypes.NameIdentifier));
            Assert.Equal(gebruiker.Email, GetClaim(ClaimTypes.Email));
            Assert.Equal(gebruiker.Rol.ToString(), GetClaim(ClaimTypes.Role));
            Assert.Equal(gebruiker.Voornaam, GetClaim(ClaimTypes.GivenName));
            Assert.Equal(gebruiker.Achternaam, GetClaim(ClaimTypes.Surname));
            Assert.Equal($"{gebruiker.Voornaam} {gebruiker.Achternaam}", GetClaim(ClaimTypes.Name));
        }

        private static JwtService CreateJwtServiceWithKey(string key)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = key
                })
                .Build();

            return new JwtService(config);
        }

        private static Gebruiker CreateGebruiker()
        {
            return new Gebruiker
            {
                Id = 123,
                Email = "sofia@test.nl",
                Voornaam = "Sofia",
                Achternaam = "Qahqai",
                Rol = UserRole.Koper,
                PasswordHash = "irrelevant.for.jwt.tests",
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}
