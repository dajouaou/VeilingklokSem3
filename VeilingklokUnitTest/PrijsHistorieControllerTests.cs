using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Veilingklok.Features.PrijsHistorie.Controllers;
using Veilingklok.Features.PrijsHistorie.Dtos;
using Veilingklok.Features.PrijsHistorie.Services;
using Xunit;

namespace VeilingklokUnitTest.Feature.PrijsHistorie
{
    // Doel: controller valideert input en roept service correct aan
    public sealed class PrijsHistorieControllerTests
    {
        [Fact]
        public async Task Get_EmptySoort_ReturnsBadRequest()
        {
            // Doel: lege input direct afkeuren (geen service-call)
            var service = new Mock<IPrijsHistorieService>();
            var controller = new PrijsHistorieController(service.Object);

            var result = await controller.Get(soort: "   ", aanvoerderId: null);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("soort is verplicht.", ((BadRequestObjectResult)result).Value);
            service.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Get_TrimsSoort_CallsServiceWithTrimmedValue_AndReturnsOk()
        {
            // Doel: spaties verwijderen en juiste service-call doen
            var service = new Mock<IPrijsHistorieService>();

            var expected = new PrijsHistorieDto
            {
                Soort = "Rozen",
                GemiddeldeAlleAanvoerders = null,
                Laatste10AlleAanvoerders = new List<PrijsPuntDto>()
            };

            service
                .Setup(s => s.GetPrijsHistorieAsync("Rozen", 3))
                .ReturnsAsync(expected);

            var controller = new PrijsHistorieController(service.Object);

            var result = await controller.Get(soort: "  Rozen  ", aanvoerderId: 3);

            Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ((OkObjectResult)result).Value);
            service.Verify(s => s.GetPrijsHistorieAsync("Rozen", 3), Times.Once);
            service.VerifyNoOtherCalls();
        }
    }

    // Doel: service faalt veilig bij foutieve configuratie
    public sealed class PrijsHistorieServiceTests
    {
        [Fact]
        public async Task GetPrijsHistorieAsync_WithoutDefaultConnection_Throws()
        {
            // Doel: zonder connection string mag geen SQL-actie starten
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            var service = new PrijsHistorieService(config);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetPrijsHistorieAsync("Rozen", huidigeAanvoerderId: null));

            Assert.Contains("DefaultConnection", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetPrijsHistorieAsync_EmptyDefaultConnection_Throws()
        {
            // Doel: lege connection string is ook ongeldig
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "   "
                })
                .Build();

            var service = new PrijsHistorieService(config);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetPrijsHistorieAsync("Rozen", huidigeAanvoerderId: null));

            Assert.Contains("DefaultConnection", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
