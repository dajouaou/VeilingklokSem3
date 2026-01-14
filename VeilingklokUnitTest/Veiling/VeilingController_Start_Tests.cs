using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    public sealed class VeilingController_Start_Tests
    {
        [Fact]
        public async Task Start_CallsService_AndReturnsOk()
        {
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            var dto = new StartVeilingDto
            {
                Veildatum = new DateTime(2026, 1, 1),
                LeverDatum = new DateTime(2026, 1, 2),
                StartTijd = new TimeSpan(9, 0, 0)
            };

            var expected = new VeilingOverzichtDto { Id = 123, IsGestart = false };

            service.Setup(s => s.StartVeilingAsync(dto.Veildatum, dto.LeverDatum, dto.StartTijd))
                   .ReturnsAsync(expected);

            var controller = new VeilingController(service.Object);

            var result = await controller.Start(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);

            service.VerifyAll();
        }

        [Fact]
        public async Task GetDetails_ReturnsOk_FromService()
        {
            var service = new Mock<IVeilingService>(MockBehavior.Strict);
            service.Setup(s => s.GetDetailsAsync(5)).ReturnsAsync(new VeilingOverzichtDto { Id = 5 });

            var controller = new VeilingController(service.Object);

            var result = await controller.GetDetails(5);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);
            Assert.Equal(5, dto.Id);

            service.VerifyAll();
        }
    }
}
