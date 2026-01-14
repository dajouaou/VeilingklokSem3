using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Xunit;

namespace VeilingklokUnitTest.Veilingmeester;

public class VeilingDashboardControllerTests
{
    // Dashboard werkt
    [Fact]
    public async Task GetDashboard_Werkt()
    {
        var svc = new Mock<IVeilingService>();
        svc.Setup(s => s.GetDetailsAsync(1)).ReturnsAsync(new VeilingOverzichtDto { Id = 1 });

        var c = new VeilingmeesterDashboardController(svc.Object);
        var res = await c.GetDashboard(1);

        var ok = Assert.IsType<OkObjectResult>(res.Result);
        var dto = Assert.IsType<VeilingmeesterDashboardDto>(ok.Value);
        Assert.Equal(1, dto.Overzicht.Id);
    }

    // Dashboard als service null geeft nog steeds object terug
    [Fact]
    public async Task GetDashboard_ServiceNull_WerktMetDefault()
    {
        var svc = new Mock<IVeilingService>();
        svc.Setup(s => s.GetDetailsAsync(1)).ReturnsAsync((VeilingOverzichtDto?)null);

        var c = new VeilingmeesterDashboardController(svc.Object);
        var res = await c.GetDashboard(1);

        var ok = Assert.IsType<OkObjectResult>(res.Result);
        var dto = Assert.IsType<VeilingmeesterDashboardDto>(ok.Value);
        Assert.NotNull(dto.Overzicht);
    }
}
