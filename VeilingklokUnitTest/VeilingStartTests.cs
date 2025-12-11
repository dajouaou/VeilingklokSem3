using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class VeilingStartTests
    {
        [TestMethod]
        public async Task Start_VerandertStatusNaarGestart()
        {
            using var db = TestDbHelper.CreateInMemory();

            var v = new Veiling { Id = 1, Status = VeilingStatus.Gepland };
            db.Veilingen.Add(v);
            db.VeilingProducten.Add(new VeilingProduct { Id = 10, VeilingId = 1, Volgorde = 1 });
            db.SaveChanges();

            var controller = new VeilingBeheerController(db, null);

            var result = await controller.Start(1);

            Assert.IsTrue(result.Value.IsGestart);
        }
    }
}
