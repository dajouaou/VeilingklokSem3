using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using Xunit;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;

namespace VeilingklokUnitTest.Veiling
{
    // Tests voor GetActief wanneer er geen gestart/gepauzeerde veiling is
    public sealed class VeilingPublicController_GetActief_NoVeiling_Tests
    {
        private static MyContext CreateDb()
        {
            // InMemory database per test
            var opts = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MyContext(opts);
        }

        [Fact]
        public async Task GetActief_NoStartedOrPausedVeiling_ReturnsEmptyOverzicht()
        {
            using var db = CreateDb();

            // Arrange: alleen een geplande veiling (dus niet actief)
            db.Veilingen.Add(new VeilingEntity
            {
                Status = VeilingStatus.Gepland,
                Datum = DateTime.Today,
                StartTijd = new TimeSpan(9, 0, 0)
            });

            await db.SaveChangesAsync();

            var controller = new VeilingPublicController(db);

            // Act
            var result = await controller.GetActief();

            // Assert: controller geeft een leeg overzicht terug
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

            Assert.Equal(0, dto.Id);
            Assert.False(dto.IsGestart);
            Assert.False(dto.IsPauze);
            Assert.False(dto.IsAfgesloten);
            Assert.Null(dto.HuidigProduct);
            Assert.NotNull(dto.Wachtrij);
            Assert.Empty(dto.Wachtrij);
        }
    }
}
