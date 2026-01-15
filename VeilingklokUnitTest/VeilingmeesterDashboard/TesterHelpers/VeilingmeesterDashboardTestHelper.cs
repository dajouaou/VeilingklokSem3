using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;

namespace VeilingklokUnitTest.Features.VeilingmeesterDashboard.TestHelpers
{
    // Helper om controllers in tests snel te maken
    public static class VeilingmeesterDashboardTestHelper
    {
        // Maakt de controller + een mock van IVeilingService
        public static (VeilingmeesterDashboardController controller, Mock<IVeilingService> serviceMock) Create()
        {
            var serviceMock = new Mock<IVeilingService>();
            var controller = new VeilingmeesterDashboardController(serviceMock.Object);
            return (controller, serviceMock);
        }
    }
}
