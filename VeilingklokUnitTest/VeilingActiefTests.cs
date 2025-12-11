using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class VeilingActiefTests
    {
        [TestMethod]
        public async Task GetActieve_GeeftActieveVeiling()
        {
            using var db = TestDbHelper.CreateInMemory();

            var v = new Veiling
            {
                Id = 10,
                Status = VeilingStatus.Gestart,
                Producten = new List<VeilingProduct>()
            };

            db.Veilingen.Add(v);
            db.SaveChanges();

            var controller = new VeilingBeheerController(db, null!);

            var result = await controller.GetActieve();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
        }
    }
}
