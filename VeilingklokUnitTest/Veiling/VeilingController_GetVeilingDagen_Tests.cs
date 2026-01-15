using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    // Unit test voor VeilingController: GetVeilingDagen
    public sealed class VeilingController_GetVeilingDagen_Tests
    {
        [Fact]
        public async Task GetVeilingDagen_ReturnsOk_ListFromService()
        {
            // Mock van de service met strikt gedrag
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            // Verwachte data die de service teruggeeft
            var expected = new List<string> { "2026-01-01", "2026-01-02" };
            service.Setup(s => s.GetVeilingDagenAsync()).ReturnsAsync(expected);

            // Controller maken met de gemockte service
            var controller = new VeilingController(service.Object);

            // Actie uitvoeren
            var result = await controller.GetVeilingDagen();

            // Controleren dat de response Ok is en dezelfde lijst bevat
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(expected, ok.Value);

            // Controleren dat alle verwachte service-calls zijn gedaan
            service.VerifyAll();
        }
    }
}
