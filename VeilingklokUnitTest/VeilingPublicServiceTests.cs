using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Features.VeilingPublic.Services;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class VeilingPublicServiceTests
    {
        private MyContext CreateDb()
        {
            return new MyContext(
                new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options
            );
        }

        [TestMethod]
        public async Task GetBeschikbareVeildagen_GeeftUniekeGesorteerdeDagen()
        {
            using var db = CreateDb();

            db.Aanmeldingen.Add(new Aanmelding { Id = 1, Veildatum = DateTime.Today });
            db.Aanmeldingen.Add(new Aanmelding { Id = 2, Veildatum = DateTime.Today.AddDays(2) });
            db.Aanmeldingen.Add(new Aanmelding { Id = 3, Veildatum = DateTime.Today }); // duplicate date
            db.SaveChanges();

            var service = new VeilingPublicService(db);

            var dagen = await service.GetBeschikbareVeildagenAsync();

            Assert.AreEqual(2, dagen.Count);
            Assert.AreEqual(DateTime.Today.ToString("yyyy-MM-dd"), dagen[0]);
        }
    }
}
