using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    // Unit tests voor de Pause, Resume en Stop acties van de VeilingController
    // Doel: controleren dat de controller correct reageert bij succes én bij fouten
    public sealed class VeilingController_PauseResumeStop_Tests
    {
        [Fact]
        public async Task Pause_CallsService_ReturnsNoContent()
        {
            // Arrange: service doet zijn werk zonder fouten
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.PauseAsync(1)).Returns(Task.CompletedTask);

            // Controller met gemockte service
            var controller = new VeilingController(service.Object);

            // Act: pauzeer de veiling
            var result = await controller.Pause(1);

            // Assert: succesvolle actie geeft NoContent terug
            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Pause_ServiceThrows_Exception_IsPropagated()
        {
            // Arrange: service gooit een fout (bijv. veiling is niet gestart)
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service
                .Setup(s => s.PauseAsync(1))
                .ThrowsAsync(new InvalidOperationException("Veiling niet gestart"));

            var controller = new VeilingController(service.Object);

            // Act + Assert: fout moet doorgegeven worden
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.Pause(1));

            service.VerifyAll();
        }

        [Fact]
        public async Task Resume_CallsService_ReturnsNoContent()
        {
            // Arrange: service hervat de veiling succesvol
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.ResumeAsync(1)).Returns(Task.CompletedTask);

            var controller = new VeilingController(service.Object);

            // Act: hervat de veiling
            var result = await controller.Resume(1);

            // Assert: succesvolle actie geeft NoContent terug
            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Resume_ServiceThrows_Exception_IsPropagated()
        {
            // Arrange: service gooit een fout (bijv. veiling is niet gepauzeerd)
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service
                .Setup(s => s.ResumeAsync(1))
                .ThrowsAsync(new InvalidOperationException("Veiling is niet gepauzeerd"));

            var controller = new VeilingController(service.Object);

            // Act + Assert: fout moet doorgegeven worden
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.Resume(1));

            service.VerifyAll();
        }

        [Fact]
        public async Task Stop_CallsService_ReturnsNoContent()
        {
            // Arrange: service stopt de veiling zonder fouten
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.StopAsync(1)).Returns(Task.CompletedTask);

            var controller = new VeilingController(service.Object);

            // Act: stop de veiling
            var result = await controller.Stop(1);

            // Assert: succesvolle actie geeft NoContent terug
            Assert.IsType<NoContentResult>(result);
            service.VerifyAll();
        }

        [Fact]
        public async Task Stop_ServiceThrows_Exception_IsPropagated()
        {
            // Arrange: service gooit een fout (bijv. veiling kan niet gestopt worden)
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service
                .Setup(s => s.StopAsync(1))
                .ThrowsAsync(new InvalidOperationException("Veiling kan niet gestopt worden"));

            var controller = new VeilingController(service.Object);

            // Act + Assert: fout moet doorgegeven worden
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.Stop(1));

            service.VerifyAll();
        }
    }
}
