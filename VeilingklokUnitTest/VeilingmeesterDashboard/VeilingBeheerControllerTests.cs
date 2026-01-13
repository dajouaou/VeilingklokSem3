using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.SignalR.Broadcasters;
using VeilingklokUnitTest.Helpers;
using Xunit;

namespace VeilingklokUnitTest.Veilingmeester;

public class VeilingBeheerControllerTests
{
    // Start faalt als veiling niet bestaat
    [Fact]
    public async Task Start_BestaatNiet_Faalt()
    {
        using var db = TestDb.Create();
        var svc = new Mock<IVeilingService>();
        var bc = new Mock<IVeilingBroadcastService>();

        var c = new VeilingBeheerController(svc.Object, bc.Object, db);

        var res = await c.Start(99);

        Assert.IsType<BadRequestObjectResult>(res.Result);
    }

    // Start faalt als starttijd in de toekomst is
    [Fact]
    public async Task Start_TeVroeg_Faalt()
    {
        using var db = TestDb.Create();
        var svc = new Mock<IVeilingService>();
        var bc = new Mock<IVeilingBroadcastService>();

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gepland, DateTime.Today.AddDays(1), new TimeSpan(9, 0, 0)));
        await db.SaveChangesAsync();

        var c = new VeilingBeheerController(svc.Object, bc.Object, db);
        var res = await c.Start(1);

        Assert.IsType<BadRequestObjectResult>(res.Result);
    }

    // Start werkt en stuurt broadcast als huidig product bestaat
    [Fact]
    public async Task Start_Werkt_Broadcast()
    {
        using var db = TestDb.Create();
        var svc = new Mock<IVeilingService>();
        var bc = new Mock<IVeilingBroadcastService>();

        db.Veilingen.Add(TestData.Veiling(1, VeilingStatus.Gepland, DateTime.Today, DateTime.Now.TimeOfDay - TimeSpan.FromMinutes(1)));
        await db.SaveChangesAsync();

        svc.Setup(s => s.StartGeplandeVeilingAsync(1))
           .ReturnsAsync(new VeilingOverzichtDto
           {
               Id = 1,
               HuidigProduct = new HuidigProductDto { VeilingProductId = 10 },
               Wachtrij = new() { new WachtrijItemDto { VeilingProductId = 11 } }
           });

        var c = new VeilingBeheerController(svc.Object, bc.Object, db);
        var res = await c.Start(1);

        Assert.IsType<OkObjectResult>(res.Result);
        bc.Verify(x => x.StuurHuidigProduct(1, It.IsAny<HuidigProductDto>()), Times.Once);
        bc.Verify(x => x.StuurWachtrij(1, It.IsAny<List<WachtrijItemDto>>()), Times.Once);
    }

    // Pause werkt en stuurt broadcast als huidig product bestaat
    [Fact]
    public async Task Pause_Werkt_Broadcast()
    {
        using var db = TestDb.Create();
        var svc = new Mock<IVeilingService>();
        var bc = new Mock<IVeilingBroadcastService>();

        svc.Setup(s => s.GetDetailsAsync(1)).ReturnsAsync(new VeilingOverzichtDto
        {
            Id = 1,
            HuidigProduct = new HuidigProductDto { VeilingProductId = 10 },
            Wachtrij = new() { new WachtrijItemDto { VeilingProductId = 11 } }
        });

        var c = new VeilingBeheerController(svc.Object, bc.Object, db);
        var res = await c.Pause(1);

        Assert.IsType<NoContentResult>(res);
        bc.Verify(x => x.StuurHuidigProduct(1, It.IsAny<HuidigProductDto>()), Times.Once);
        bc.Verify(x => x.StuurWachtrij(1, It.IsAny<List<WachtrijItemDto>>()), Times.Once);
    }

    // Resume werkt en stuurt broadcast als huidig product bestaat
    [Fact]
    public async Task Resume_Werkt_Broadcast()
    {
        using var db = TestDb.Create();
        var svc = new Mock<IVeilingService>();
        var bc = new Mock<IVeilingBroadcastService>();

        svc.Setup(s => s.GetDetailsAsync(1)).ReturnsAsync(new VeilingOverzichtDto
        {
            Id = 1,
            HuidigProduct = new HuidigProductDto { VeilingProductId = 10 },
            Wachtrij = new() { new WachtrijItemDto { VeilingProductId = 11 } }
        });

        var c = new VeilingBeheerController(svc.Object, bc.Object, db);
        var res = await c.Resume(1);

        Assert.IsType<NoContentResult>(res);
        bc.Verify(x => x.StuurHuidigProduct(1, It.IsAny<HuidigProductDto>()), Times.Once);
        bc.Verify(x => x.StuurWachtrij(1, It.IsAny<List<WachtrijItemDto>>()), Times.Once);
    }

    // Stop werkt
    [Fact]
    public async Task Stop_Werkt()
    {
        using var db = TestDb.Create();
        var svc = new Mock<IVeilingService>();
        var bc = new Mock<IVeilingBroadcastService>();

        var c = new VeilingBeheerController(svc.Object, bc.Object, db);
        var res = await c.Stop(1);

        Assert.IsType<NoContentResult>(res);
        svc.Verify(s => s.StopAsync(1), Times.Once);
    }
}
