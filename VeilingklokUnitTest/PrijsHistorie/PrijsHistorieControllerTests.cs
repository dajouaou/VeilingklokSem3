using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Veilingklok.Features.PrijsHistorie.Services;
using Xunit;
namespace VeilingklokUnitTest.Features.PrijsHistorie
{
    // Deze class bevat alle tests voor PrijsHistorieService
    public sealed class PrijsHistorieServiceTests
    {
        [Fact]
        public async Task GetPrijsHistorieAsync_WithoutDefaultConnection_ThrowsException()
        {
            // We maken een configuratie aan ZONDER enige instellingen
            // Dus: géén connection string aanwezig
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            // We maken de service aan die we willen testen
            // Deze service gebruikt de configuratie
            var service = new PrijsHistorieService(config);

            // We verwachten dat er een Exception wordt gegooid
            // omdat er geen DefaultConnection bestaat
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetPrijsHistorieAsync("Rozen", null));

            // We controleren of de foutmelding het woord "DefaultConnection" bevat
            // Dit laat zien dat de fout duidelijk over de ontbrekende connection gaat
            Assert.Contains("DefaultConnection", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        // Nog een aparte test
        // Deze test controleert wat er gebeurt als de connection string WEL bestaat,
        // maar leeg of alleen spaties bevat
        [Fact]
        public async Task GetPrijsHistorieAsync_EmptyDefaultConnection_ThrowsException()
        {
            // We maken een configuratie met een DefaultConnection
            // maar de waarde is leeg (alleen spaties)
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "   "
                })
                .Build();

            // We maken opnieuw de service aan
            var service = new PrijsHistorieService(config);

            // We verwachten opnieuw een Exception
            // omdat een lege connection string ongeldig is
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetPrijsHistorieAsync("Rozen", null));

            // We controleren weer of de foutmelding duidelijk verwijst
            // naar de DefaultConnection
            Assert.Contains("DefaultConnection", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
