using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Xunit;
using VeilingEntity = Veilingklok.Core.Entities.Veiling;

namespace VeilingklokUnitTest.Veiling
{
    // Tests voor VeilingPublicController.GetVolgende (eerstvolgende geplande veiling)
    public sealed class VeilingPublicController_GetVolgende_Tests
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
        public async Task GetVolgende_NoPlannedVeiling_ReturnsNull()
        {
            using var db = CreateDb();
            var controller = new VeilingPublicController(db);

            // Act
            var result = await controller.GetVolgende();

            // Assert: Ok(null)
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Null(ok.Value);
        }

        [Fact]
        public async Task GetVolgende_PlannedVeiling_ReturnsDto_WithCount()
        {
            using var db = CreateDb();

            // Arrange: geplande veiling voor morgen
            var v = new VeilingEntity
            {
                Id = 1,
                Status = VeilingStatus.Gepland,
                Datum = DateTime.Today.AddDays(1),
                StartTijd = new TimeSpan(9, 0, 0)
            };
            db.Veilingen.Add(v);

            // Arrange: 2 producten gekoppeld aan deze veiling
            db.VeilingProducten.Add(new VeilingProduct { Id = 10, VeilingId = 1 });
            db.VeilingProducten.Add(new VeilingProduct { Id = 11, VeilingId = 1 });

            await db.SaveChangesAsync();

            var controller = new VeilingPublicController(db);

            // Act
            var result = await controller.GetVolgende();

            // Assert: juiste dto + juiste aantallen
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<GeplandeVeilingListItemDto>(ok.Value);

            Assert.Equal(1, dto.Id);
            Assert.Equal(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), dto.Veildatum);
            Assert.Equal("09:00", dto.StartTijd);
            Assert.Equal(2, dto.AantalProducten);
        }
    }
}
