using System;
using Veilingklok.Features.Auth.Services;
using Xunit;

namespace VeilingklokUnitTest.Auth
{
    public sealed class PasswordServiceTests
    {
        // System Under Test
        private readonly PasswordService _sut = new();

        [Fact]
        public void HashPassword_ReturnsSaltDotHashFormat()
        {
            var password = "Test123!";

            var hashed = _sut.HashPassword(password);

            Assert.False(string.IsNullOrWhiteSpace(hashed));

            var parts = hashed.Split('.');
            Assert.Equal(2, parts.Length);

            Convert.FromBase64String(parts[0]);
            Convert.FromBase64String(parts[1]);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var password = "Test123!";
            var hashed = _sut.HashPassword(password);

            var ok = _sut.VerifyPassword(password, hashed);

            Assert.True(ok);
        }

        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var password = "Test123!";
            var hashed = _sut.HashPassword(password);

            var ok = _sut.VerifyPassword("WrongPassword!", hashed);

            Assert.False(ok);
        }

        [Fact]
        public void HashPassword_SameInput_GivesDifferentHash()
        {
            var password = "Test123!";

            var hashed1 = _sut.HashPassword(password);
            var hashed2 = _sut.HashPassword(password);

            Assert.NotEqual(hashed1, hashed2);

            var parts1 = hashed1.Split('.');
            var parts2 = hashed2.Split('.');

            Assert.Equal(2, parts1.Length);
            Assert.Equal(2, parts2.Length);

            Assert.NotEqual(parts1[0], parts2[0]);
            Assert.NotEqual(parts1[1], parts2[1]);
        }
    }
}
