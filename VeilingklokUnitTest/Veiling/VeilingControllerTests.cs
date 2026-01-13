using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Xunit;

namespace VeilingklokUnitTest.Veiling;

public class VeilingControllerTests
{
    private readonly Mock<IVeilingService> _svc = new();

    [Fact]
    public async Task Start_Ok()
    {
        var expected = new VeilingOverzichtDto { Id = 1, IsGestart = true };
        _svc.Setup(s => s.StartVeilingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan?>()))
            .ReturnsAsync(expected);

        var c = new VeilingController(_svc.Object);
        var dto = new StartVeilingDto { Veildatum = DateTime.Today, LeverDatum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) };

        var result = await c.Start(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        _svc.Verify(s => s.StartVeilingAsync(dto.Veildatum, dto.LeverDatum, dto.StartTijd), Times.Once);
    }

    [Fact]
    public async Task GetDetails_Ok()
    {
        _svc.Setup(s => s.GetDetailsAsync(5)).ReturnsAsync(new VeilingOverzichtDto { Id = 5 });

        var c = new VeilingController(_svc.Object);
        var result = await c.GetDetails(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);
        Assert.Equal(5, dto.Id);
    }

    [Fact]
    public async Task Pause_NoContent()
    {
        var c = new VeilingController(_svc.Object);
        var result = await c.Pause(2);

        Assert.IsType<NoContentResult>(result);
        _svc.Verify(s => s.PauseAsync(2), Times.Once);
    }

    [Fact]
    public async Task Resume_NoContent()
    {
        var c = new VeilingController(_svc.Object);
        var result = await c.Resume(2);

        Assert.IsType<NoContentResult>(result);
        _svc.Verify(s => s.ResumeAsync(2), Times.Once);
    }

    [Fact]
    public async Task Stop_NoContent()
    {
        var c = new VeilingController(_svc.Object);
        var result = await c.Stop(2);

        Assert.IsType<NoContentResult>(result);
        _svc.Verify(s => s.StopAsync(2), Times.Once);
    }

    [Fact]
    public async Task GetVeilingDagen_OkList()
    {
        var days = new List<string> { "2026-01-01", "2026-01-02" };
        _svc.Setup(s => s.GetVeilingDagenAsync()).ReturnsAsync(days);

        var c = new VeilingController(_svc.Object);
        var result = await c.GetVeilingDagen();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(days, ok.Value);
    }
}
