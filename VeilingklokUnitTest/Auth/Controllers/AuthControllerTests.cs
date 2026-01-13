using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Controllers;
using Veilingklok.Features.Auth.Dtos;
using Veilingklok.Features.Auth.Services;
using Xunit;

namespace VeilingklokUnitTest.Auth.Controllers
{
    // Controller tests: hier mock ik alleen de repository, en gebruik ik echte AuthService.
    public class AuthControllerTests
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

        private static AuthController MakeController(Mock<IGebruikerRepository> repo)
        {
            var password = new PasswordService();
            var jwt = new JwtService(TestConfig());
            var authService = new AuthService(repo.Object, password, jwt);

            return new AuthController(authService, repo.Object);
        }

        [Fact]
        public async Task Register_UserBestaatAl_ReturnsBadRequest()
        {
            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync("a@b.nl"))
                .ReturnsAsync(new Gebruiker { Id = 1, Email = "a@b.nl" });

            var controller = MakeController(repo);

            var dto = new RegisterRequest
            {
                Email = "a@b.nl",
                Password = "pw",
                Voornaam = "Jan",
                Achternaam = "Janssen",
                Rol = UserRole.Koper
            };

            var result = await controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_OngeldigeRol_ReturnsBadRequest()
        {
            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Gebruiker?)null);

            var controller = MakeController(repo);

            var dto = new RegisterRequest
            {
                Email = "x@x.nl",
                Password = "pw",
                Voornaam = "A",
                Achternaam = "B",
                Rol = UserRole.Admin // jouw controller accepteert dit niet
            };

            var result = await controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_Valid_ReturnsOk()
        {
            var repo = new Mock<IGebruikerRepository>();

            // Controller check: bestaat gebruiker al?
            repo.Setup(r => r.GetByEmailAsync("nieuw@b.nl"))
                .ReturnsAsync((Gebruiker?)null);

            // Service register: AddAsync + create koper
            repo.Setup(r => r.AddAsync(It.IsAny<Gebruiker>()))
                .Returns(Task.CompletedTask)
                .Callback<Gebruiker>(g => g.Id = 50);

            repo.Setup(r => r.CreateKoperAsync(It.IsAny<Koper>()))
                .Returns(Task.CompletedTask);

            var controller = MakeController(repo);

            var dto = new RegisterRequest
            {
                Email = "nieuw@b.nl",
                Password = "pw",
                Voornaam = "Jan",
                Achternaam = "Janssen",
                Rol = UserRole.Koper
            };

            var result = await controller.Register(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_GebruikerNietGevonden_ReturnsBadRequest()
        {
            var repo = new Mock<IGebruikerRepository>();
            repo.Setup(r => r.GetByEmailAsync("x@x.nl"))
                .ReturnsAsync((Gebruiker?)null);

            var controller = MakeController(repo);

            var result = await controller.Login(new LoginRequest("x@x.nl", "pw"));

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_Geldig_ReturnsOk()
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

            // controller maakt zijn eigen AuthService, dus die gebruikt ook repo.GetByEmailAsync en password verify
            var controller = MakeController(repo);

            var result = await controller.Login(new LoginRequest("x@x.nl", "pw"));

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
