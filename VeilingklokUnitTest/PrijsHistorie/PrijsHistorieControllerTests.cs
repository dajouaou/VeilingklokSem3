using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Veilingklok.Features.PrijsHistorie.Services;
using Xunit;

namespace VeilingklokUnitTest.Features.PrijsHistorie
{
    public sealed class PrijsHistorieServiceTests
    {
        [Fact]
        public async Task GetPrijsHistorieAsync_WithoutDefaultConnection_ThrowsException()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            var service = new PrijsHistorieService(config);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetPrijsHistorieAsync("Rozen", null));

            Assert.Contains("DefaultConnection", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetPrijsHistorieAsync_EmptyDefaultConnection_ThrowsException()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "   "
                })
                .Build();

            var service = new PrijsHistorieService(config);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetPrijsHistorieAsync("Rozen", null));

            Assert.Contains("DefaultConnection", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
