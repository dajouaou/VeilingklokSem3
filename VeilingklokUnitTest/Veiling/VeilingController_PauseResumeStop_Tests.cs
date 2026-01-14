using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    public sealed class VeilingController_PauseResumeStop_Tests
    {
        [Fact]
        public async Task Pause_CallsService_ReturnsNoContent()
        {
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.PauseAsync(1)).Returns(Task.CompletedTask);

            var controller = new VeilingController(service.Object);

            var result = await controller.Pause(1);

            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Resume_CallsService_ReturnsNoContent()
        {
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.ResumeAsync(1)).Returns(Task.CompletedTask);

            var controller = new VeilingController(service.Object);

            var result = await controller.Resume(1);

            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Stop_CallsService_ReturnsNoContent()
        {
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.StopAsync(1)).Returns(Task.CompletedTask);

            var controller = new VeilingController(service.Object);

            var result = await controller.Stop(1);

            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }
    }
}
