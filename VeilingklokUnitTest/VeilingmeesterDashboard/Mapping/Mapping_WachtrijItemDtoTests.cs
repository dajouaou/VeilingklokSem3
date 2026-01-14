using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Veiling.Dtos;
using VeilingklokUnitTest.Features.VeilingmeesterDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.Features.VeilingmeesterDashboard.Mapping
{
    public class Mapping_WachtrijItemDtoTests
    {
        [Fact]
        public void Map_VeilingProductNaarWachtrijItemDto_NeemtAlleVeldenOver()
        {
            // Arrange: AutoMapper en een voorbeeld product in de wachtrij
            var mapper = AutoMapperTestHelper.CreateMapper();

            var product = new VeilingProduct
            {
                Id = 100,
                ResterendeHoeveelheid = 10,
                Aanmelding = new Aanmelding
                {
                    Soort = "Komkommer",
                    FotoUrl = "komkommer.png",
                    AanvoerderId = 8,
                    Aanvoerder = new Aanvoerder { Naam = "Boer Piet" }
                }
            };

            // Act: map het entity object naar het DTO
            var dto = mapper.Map<WachtrijItemDto>(product);

            // Assert: controleer of alle velden juist zijn gemapt
            Assert.Equal("Komkommer", dto.Soort);
            Assert.Equal("komkommer.png", dto.FotoUrl);
            Assert.Equal(10, dto.ResterendeHoeveelheid);
            Assert.Equal(100, dto.VeilingProductId);
            Assert.Equal(8, dto.AanvoerderId);
            Assert.Equal("Boer Piet", dto.AanvoerderNaam);
        }
    }
}
