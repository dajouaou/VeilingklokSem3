using System;
using Veilingklok.Features.Auth.Services;
using Xunit;

namespace VeilingklokUnitTest.Feature.Auth
{
    public sealed class PasswordServiceTests
    {
        // System Under Test: de echte PasswordService
        private readonly PasswordService _sut = new();

        // Test: HashPassword geeft het juiste formaat terug
        // Doel: zeker weten dat de hash bestaat uit salt.hash
        [Fact]
        public void HashPassword_ReturnsSaltDotHashFormat()
        {
            var password = "Test123!";

            var hashed = _sut.HashPassword(password);

            // Hash mag niet leeg zijn
            Assert.False(string.IsNullOrWhiteSpace(hashed));

            // Verwacht precies twee delen: salt en hash
            var parts = hashed.Split('.');
            Assert.Equal(2, parts.Length);

            // Beide delen moeten geldige Base64 zijn
            Convert.FromBase64String(parts[0]);
            Convert.FromBase64String(parts[1]);
        }

        // Test: juiste wachtwoord wordt geaccepteerd
        // Doel: login mag slagen met correct wachtwoord
        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var password = "Test123!";
            var hashed = _sut.HashPassword(password);

            var ok = _sut.VerifyPassword(password, hashed);

            Assert.True(ok);
        }

        // Test: fout wachtwoord wordt geweigerd
        // Doel: voorkomen dat iemand met verkeerd wachtwoord inlogt
        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var password = "Test123!";
            var hashed = _sut.HashPassword(password);

            var ok = _sut.VerifyPassword("WrongPassword!", hashed);

            Assert.False(ok);
        }

        // Test: dezelfde input geeft verschillende hashes
        // Doel: controleren dat random salt wordt gebruikt
        [Fact]
        public void HashPassword_SameInput_GivesDifferentHash()
        {
            var password = "Test123!";

            var hashed1 = _sut.HashPassword(password);
            var hashed2 = _sut.HashPassword(password);

            // Volledige hash moet verschillen
            Assert.NotEqual(hashed1, hashed2);

            var parts1 = hashed1.Split('.');
            var parts2 = hashed2.Split('.');

            Assert.Equal(2, parts1.Length);
            Assert.Equal(2, parts2.Length);

            // Salt en hash moeten beide verschillen
            Assert.NotEqual(parts1[0], parts2[0]);
            Assert.NotEqual(parts1[1], parts2[1]);
        }
    }
}
