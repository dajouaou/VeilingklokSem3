using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    // Unit tests voor pause, resume en stop acties van de VeilingController
    public sealed class VeilingController_PauseResumeStop_Tests
    {
        [Fact]
        public async Task Pause_CallsService_ReturnsNoContent()
        {
            // Mock van de service
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.PauseAsync(1)).Returns(Task.CompletedTask);

            // Controller met mock service
            var controller = new VeilingController(service.Object);

            // Actie uitvoeren
            var result = await controller.Pause(1);

            // Controleren dat NoContent wordt teruggegeven
            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Resume_CallsService_ReturnsNoContent()
        {
            // Mock van de service
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.ResumeAsync(1)).Returns(Task.CompletedTask);

            // Controller met mock service
            var controller = new VeilingController(service.Object);

            // Actie uitvoeren
            var result = await controller.Resume(1);

            // Controleren dat NoContent wordt teruggegeven
            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Stop_CallsService_ReturnsNoContent()
        {
            // Mock van de service
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.StopAsync(1)).Returns(Task.CompletedTask);

            // Controller met mock service
            var controller = new VeilingController(service.Object);

            // Actie uitvoeren
            var result = await controller.Stop(1);

            // Controleren dat NoContent wordt teruggegeven
            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }
    }
}
