using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Services;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class AuthLoginTests
    {
        [TestMethod]
        public async Task Login_GeldigeGebruiker_GeeftToken()
        {
            // Arrange
            var gebruikerRepo = new Mock<IGebruikerRepository>();
            var password = new Mock<IPasswordService>();
            var jwt = new Mock<IJwtService>();

            var user = new Gebruiker
            {
                Id = 1,
                Email = "test@mail.com",
                Rol = UserRole.Koper,
                PasswordHash = "HASH"
            };

            gebruikerRepo
                .Setup(r => r.GetByEmailAsync("test@mail.com"))
                .ReturnsAsync(user);

            password
                .Setup(p => p.VerifyPassword("1234", "HASH"))
                .Returns(true);

            jwt
                .Setup(j => j.GenerateToken(user))
                .Returns("FAKE_TOKEN");

            var service = new AuthService(gebruikerRepo.Object, password.Object, jwt.Object);

            // Act
            string token = await service.LoginAsync("test@mail.com", "1234");

            // Assert
            Assert.AreEqual("FAKE_TOKEN", token);
        }
    }
}

