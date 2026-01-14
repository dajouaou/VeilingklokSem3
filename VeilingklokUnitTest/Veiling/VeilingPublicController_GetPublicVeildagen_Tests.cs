using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Infrastructure.Database;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    public sealed class VeilingPublicController_GetPublicVeildagen_Tests
    {
        private static MyContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MyContext(opts);
        }

        [Fact]
        public async Task GetPublicVeildagen_ReturnsDistinctSorted_OnlyWhereVeilingProductIdNull()
        {
            using var db = CreateDb();

            db.Aanmeldingen.Add(new Aanmelding { LeverDatum = DateTime.Today, VeilingProductId = null });
            db.Aanmeldingen.Add(new Aanmelding { LeverDatum = DateTime.Today.AddDays(1), VeilingProductId = null });
            db.Aanmeldingen.Add(new Aanmelding { LeverDatum = DateTime.Today, VeilingProductId = null }); // duplicate
            db.Aanmeldingen.Add(new Aanmelding { LeverDatum = DateTime.Today.AddDays(2), VeilingProductId = 999 }); // moet eruit

            await db.SaveChangesAsync();

            var controller = new VeilingPublicController(db);

            var result = await controller.GetPublicVeildagen();

            var list = result.Value!;
            Assert.Equal(2, list.Count);
            Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), list[0]);
            Assert.Equal(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), list[1]);
            Assert.True(list.SequenceEqual(list.OrderBy(x => x)));
        }
    }
}
