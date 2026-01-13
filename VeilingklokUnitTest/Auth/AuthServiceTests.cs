using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Services;
using Xunit;

namespace VeilingklokUnitTest.Auth
{
    // Hier mock ik alleen de repository. De rest (AuthService/Password/Jwt) is echte code.
    public class AuthServiceTests
    {
        private static IConfiguration TestConfig()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string?>
            {
                ["Jwt:Key"] = "DIT_IS_EEN_TEST_KEY_MET_GENOEg_LENGTE_1234567890"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict!)
                .Build();
        }

        [Fact]
        public async Task Register_EmailBestaatAl_Throws()
        {
            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync("a@b.nl"))
                .ReturnsAsync(new Gebruiker { Id = 1, Email = "a@b.nl" });

            var password = new PasswordService();
            var jwt = new JwtService(TestConfig());
            var service = new AuthService(repo.Object, password, jwt);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.RegisterAsync("a@b.nl", "pw", "Jan", "Janssen", UserRole.Koper));

            Assert.Contains("Email is al in gebruik", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_Valid_Koper_CreatesKoper_AndReturnsToken()
        {
            var repo = new Mock<IGebruikerRepository>();

            // Email bestaat nog niet (wordt 2x aangeroepen: controller/service in echte flow, hier alleen service)
            repo.Setup(r => r.GetByEmailAsync("nieuw@b.nl"))
                .ReturnsAsync((Gebruiker?)null);

            // Als AddAsync gebeurt, doen we alsof de DB een id teruggeeft
            repo.Setup(r => r.AddAsync(It.IsAny<Gebruiker>()))
                .Returns(Task.CompletedTask)
                .Callback<Gebruiker>(g => g.Id = 123);

            repo.Setup(r => r.CreateKoperAsync(It.IsAny<Koper>()))
                .Returns(Task.CompletedTask);

            var password = new PasswordService();
            var jwt = new JwtService(TestConfig());
            var service = new AuthService(repo.Object, password, jwt);

            var token = await service.RegisterAsync("nieuw@b.nl", "pw", "Jan", "Janssen", UserRole.Koper);

            Assert.False(string.IsNullOrWhiteSpace(token));

            repo.Verify(r => r.CreateKoperAsync(It.Is<Koper>(k =>
                k.GebruikerId == 123 &&
                k.Naam == "Jan Janssen"
            )), Times.Once);
        }

        [Fact]
        public async Task Login_UserNotFound_Throws()
        {
            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync("x@x.nl"))
                .ReturnsAsync((Gebruiker?)null);

            var password = new PasswordService();
            var jwt = new JwtService(TestConfig());
            var service = new AuthService(repo.Object, password, jwt);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.LoginAsync("x@x.nl", "pw"));
            Assert.Contains("Ongeldige login", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_WrongPassword_Throws()
        {
            var password = new PasswordService();
            var hashVanAnderWachtwoord = password.HashPassword("ANDER");

            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync("x@x.nl"))
                .ReturnsAsync(new Gebruiker
                {
                    Id = 1,
                    Email = "x@x.nl",
                    Rol = UserRole.Koper,
                    PasswordHash = hashVanAnderWachtwoord
                });

            var jwt = new JwtService(TestConfig());
            var service = new AuthService(repo.Object, password, jwt);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.LoginAsync("x@x.nl", "pw"));
            Assert.Contains("Ongeldige login", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Login_Valid_ReturnsToken()
        {
            var password = new PasswordService();
            var hash = password.HashPassword("pw");

            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync("x@x.nl"))
                .ReturnsAsync(new Gebruiker
                {
                    Id = 2,
                    Email = "x@x.nl",
                    Rol = UserRole.Koper,
                    Voornaam = "A",
                    Achternaam = "B",
                    PasswordHash = hash
                });

            var jwt = new JwtService(TestConfig());
            var service = new AuthService(repo.Object, password, jwt);

            var token = await service.LoginAsync("x@x.nl", "pw");

            Assert.False(string.IsNullOrWhiteSpace(token));
        }
    }
}
