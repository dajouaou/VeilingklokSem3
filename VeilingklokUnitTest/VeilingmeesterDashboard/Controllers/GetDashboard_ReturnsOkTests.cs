using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using VeilingklokUnitTest.Features.VeilingmeesterDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.Features.VeilingmeesterDashboard.Controllers
{
    public class GetDashboard_ReturnsOkTests
    {
        [Fact]
        public async Task GetDashboard_GeldigeServiceReturn_GeeftOkMetDashboardDto()
        {
            // Arrange: controller + service mock
            var (controller, serviceMock) = VeilingmeesterDashboardTestHelper.Create();

            // Service geeft een "overzicht" terug
            var overzicht = new VeilingOverzichtDto();
            serviceMock.Setup(s => s.GetDetailsAsync(5)).ReturnsAsync(overzicht);

            // Act: 1 methode aanroepen (zoals in de slides)
            var result = await controller.GetDashboard(5);

            // Assert: OkObjectResult met een VeilingmeesterDashboardDto
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<VeilingmeesterDashboardDto>(ok.Value);

            // Overzicht komt uit de service
            Assert.Same(overzicht, dto.Overzicht);

            // Lijsten worden altijd leeg teruggegeven (volgens controller code)
            Assert.Empty(dto.Biedingen);
            Assert.Empty(dto.AuditEvents);

            // Controle: service is precies 1 keer aangeroepen met het juiste id
            serviceMock.Verify(s => s.GetDetailsAsync(5), Times.Once);
        }
    }
}
