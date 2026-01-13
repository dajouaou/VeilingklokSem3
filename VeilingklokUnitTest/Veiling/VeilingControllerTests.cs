using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Xunit;

namespace VeilingklokUnitTest.Veiling;

public class VeilingControllerTests
{
    // Start geeft Ok terug
    [Fact]
    public async Task Start_Werkt_ReturnsOk()
    {
        var service = new Mock<IVeilingService>();
        service.Setup(s => s.StartVeilingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan?>()))
               .ReturnsAsync(new VeilingOverzichtDto { Id = 1 });

        var controller = new VeilingController(service.Object);
        var dto = new StartVeilingDto { Veildatum = DateTime.Today, LeverDatum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) };

        var result = await controller.Start(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<VeilingOverzichtDto>(ok.Value);
        Assert.Equal(1, body.Id);
    }

    // Details geeft Ok terug
    [Fact]
    public async Task GetDetails_Werkt_ReturnsOk()
    {
        var service = new Mock<IVeilingService>();
        service.Setup(s => s.GetDetailsAsync(5)).ReturnsAsync(new VeilingOverzichtDto { Id = 5 });

        var controller = new VeilingController(service.Object);
        var result = await controller.GetDetails(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<VeilingOverzichtDto>(ok.Value);
        Assert.Equal(5, body.Id);
    }

    // Pause geeft NoContent terug
    [Fact]
    public async Task Pause_Werkt_ReturnsNoContent()
    {
        var service = new Mock<IVeilingService>();
        var controller = new VeilingController(service.Object);

        var result = await controller.Pause(1);

        Assert.IsType<NoContentResult>(result);
        service.Verify(s => s.PauseAsync(1), Times.Once);
    }

    // Resume geeft NoContent terug
    [Fact]
    public async Task Resume_Werkt_ReturnsNoContent()
    {
        var service = new Mock<IVeilingService>();
        var controller = new VeilingController(service.Object);

        var result = await controller.Resume(1);

        Assert.IsType<NoContentResult>(result);
        service.Verify(s => s.ResumeAsync(1), Times.Once);
    }

    // Stop geeft NoContent terug
    [Fact]
    public async Task Stop_Werkt_ReturnsNoContent()
    {
        var service = new Mock<IVeilingService>();
        var controller = new VeilingController(service.Object);

        var result = await controller.Stop(1);

        Assert.IsType<NoContentResult>(result);
        service.Verify(s => s.StopAsync(1), Times.Once);
    }

    // Dagen geeft Ok terug
    [Fact]
    public async Task GetVeilingDagen_Werkt_ReturnsOk()
    {
        var service = new Mock<IVeilingService>();
        service.Setup(s => s.GetVeilingDagenAsync()).ReturnsAsync(new List<string> { "2026-01-10" });

        var controller = new VeilingController(service.Object);
        var result = await controller.GetVeilingDagen();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<List<string>>(ok.Value);
        Assert.Single(body);
    }
}
