using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using VeilingklokUnitTest.Features.VeilingmeesterDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.Features.VeilingmeesterDashboard.Controllers
{
    public class GetDashboard_NullOverzichtTests
    {
        [Fact]
        public async Task GetDashboard_ServiceGeeftNull_GeeftDefaultOverzichtTerug()
        {
            // Arrange
            var (controller, serviceMock) = VeilingmeesterDashboardTestHelper.Create();

            // We forceren hier null om te testen wat de controller doet als de service niets teruggeeft
            serviceMock.Setup(s => s.GetDetailsAsync(1))
                .ReturnsAsync((Veilingklok.Features.Veiling.Dtos.VeilingOverzichtDto)null!);


            // Act
            var result = await controller.GetDashboard(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<VeilingmeesterDashboardDto>(ok.Value);

            // Controller maakt zelf een nieuw overzicht als service null geeft
            Assert.NotNull(dto.Overzicht);

            // Lijsten zijn leeg
            Assert.Empty(dto.Biedingen);
            Assert.Empty(dto.AuditEvents);
        }
    }
}
