using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Veiling.Dtos;
using VeilingklokUnitTest.Features.VeilingmeesterDashboard.TestHelpers;
using Xunit;

namespace VeilingklokUnitTest.Features.VeilingmeesterDashboard.Mapping
{
    public class Mapping_HuidigProductDtoTests
    {
        [Fact]
        public void Map_VeilingProductNaarHuidigProductDto_NeemtAlleVeldenOver()
        {
            // Arrange: AutoMapper en een voorbeeld veilingproduct
            var mapper = AutoMapperTestHelper.CreateMapper();

            var product = new VeilingProduct
            {
                Id = 99,
                ResterendeHoeveelheid = 25,
                Aanmelding = new Aanmelding
                {
                    Soort = "Tomaat",
                    FotoUrl = "foto.jpg",
                    AanvoerderId = 7,
                    Aanvoerder = new Aanvoerder { Naam = "Boer Jan" }
                }
            };

            // Act: map het entity object naar het DTO
            var dto = mapper.Map<HuidigProductDto>(product);

            // Assert: controleer of alle velden correct zijn overgenomen
            Assert.Equal("Tomaat", dto.Soort);
            Assert.Equal("foto.jpg", dto.FotoUrl);
            Assert.Equal(25, dto.ResterendeHoeveelheid);
            Assert.Equal(99, dto.VeilingProductId);
            Assert.Equal(7, dto.AanvoerderId);
            Assert.Equal("Boer Jan", dto.AanvoerderNaam);
        }
    }
}
