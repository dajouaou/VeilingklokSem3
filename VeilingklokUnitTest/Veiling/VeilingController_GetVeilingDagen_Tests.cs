using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    public sealed class VeilingController_GetVeilingDagen_Tests
    {
        [Fact]
        public async Task GetVeilingDagen_ReturnsOk_ListFromService()
        {
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            var expected = new List<string> { "2026-01-01", "2026-01-02" };
            service.Setup(s => s.GetVeilingDagenAsync()).ReturnsAsync(expected);

            var controller = new VeilingController(service.Object);

            var result = await controller.GetVeilingDagen();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(expected, ok.Value);

            service.VerifyAll();
        }
    }
}
