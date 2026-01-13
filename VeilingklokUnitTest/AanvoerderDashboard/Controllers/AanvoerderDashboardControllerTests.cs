using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.AanvoerderDashboard.Controllers;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using VeilingklokUnitTest.AanvoerderDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.AanvoerderDashboard.Controllers
{
    // Simpele controller tests: hier mock ik de service en test ik alleen controller-gedrag (statuscodes).
    public class AanvoerderDashboardControllerTests
    {
        private static AanvoerderDashboardController MakeController(
            Mock<IAanvoerderDashboardService> serviceMock,
            ClaimsPrincipal? user = null)
        {
            // echte in-memory db, want controller ctor verwacht MyContext (we gebruiken hem niet in deze tests)
            var db = AanvoerderDashboardTestFactory.CreateDb();

            // env is verplicht in ctor, maar we gebruiken hem hier niet
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns("C:\\temp");

            var controller = new AanvoerderDashboardController(serviceMock.Object, db, envMock.Object);

            // HttpContext zetten zodat User beschikbaar is
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            if (user != null)
                controller.ControllerContext.HttpContext.User = user;

            return controller;
        }

        private static ClaimsPrincipal UserWithId(int gebruikerId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, gebruikerId.ToString()) };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        // ---------------- GET aanmeldingen ----------------

        [Fact]
        public async Task GetAanmeldingen_ZonderUserId_IsUnauthorized()
        {
            var serviceMock = new Mock<IAanvoerderDashboardService>();
            var controller = MakeController(serviceMock); // geen user -> geen claim

            var result = await controller.GetAanmeldingen(null);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAanmeldingen_ServiceOk_GeeftOk()
        {
            var serviceMock = new Mock<IAanvoerderDashboardService>();
            serviceMock
                .Setup(s => s.GetAanmeldingenAsync(5, null))
                .ReturnsAsync(new List<AanmeldingListItemDto>
                {
                    new AanmeldingListItemDto { Id = 1, Soort = "Roos" }
                });

            var controller = MakeController(serviceMock, UserWithId(5));

            var result = await controller.GetAanmeldingen(null);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<AanmeldingListItemDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetAanmeldingen_ServiceGooiException_GeeftNotFound()
        {
            var serviceMock = new Mock<IAanvoerderDashboardService>();
            serviceMock
                .Setup(s => s.GetAanmeldingenAsync(6, null))
                .ThrowsAsync(new ArgumentException("Geen aanvoerder-profiel gevonden."));

            var controller = MakeController(serviceMock, UserWithId(6));

            var result = await controller.GetAanmeldingen(null);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ---------------- DELETE aanmelding ----------------

        [Fact]
        public async Task DeleteAanmelding_ServiceOk_GeeftNoContent()
        {
            var serviceMock = new Mock<IAanvoerderDashboardService>();
            serviceMock
                .Setup(s => s.DeleteAanmeldingAsync(7, 10))
                .Returns(Task.CompletedTask);

            var controller = MakeController(serviceMock, UserWithId(7));

            var result = await controller.DeleteAanmelding(10);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAanmelding_NietGevonden_GeeftNotFound()
        {
            var serviceMock = new Mock<IAanvoerderDashboardService>();
            serviceMock
                .Setup(s => s.DeleteAanmeldingAsync(8, 99))
                .ThrowsAsync(new ArgumentException("Aanmelding niet gevonden."));

            var controller = MakeController(serviceMock, UserWithId(8));

            var result = await controller.DeleteAanmelding(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
