using AutoMapper;
using Veilingklok.Features.VeilingmeesterDashboard.Mapping;

namespace VeilingklokUnitTest.Features.VeilingmeesterDashboard.TestHelpers
{
    // Helper om AutoMapper te maken voor mapping tests
    public static class AutoMapperTestHelper
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VeilingDashboardMappingProfile>();
            });

            // Controle of mapping geldig is (foutmelding als mapping verkeerd is)
            config.AssertConfigurationIsValid();

            return config.CreateMapper();
        }
    }
}
