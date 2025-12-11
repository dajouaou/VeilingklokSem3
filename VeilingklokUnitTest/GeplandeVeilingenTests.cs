using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class GeplandeVeilingenTests
    {
        [TestMethod]
        public async Task GetGeplande_GeeftLijstTerug()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Veilingen.Add(new Veiling
            {
                Id = 1,
                Datum = DateTime.Today,
                Status = VeilingStatus.Gepland,
                Producten = new List<VeilingProduct>()
            });
            db.SaveChanges();

            var controller = new VeilingPlanningController(db);

            var result = await controller.GetGeplande();
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);
            var list = ok.Value as List<Veilingklok.Features.VeilingmeesterDashboard.Dtos.GeplandeVeilingListItemDto>;
            Assert.AreEqual(1, list.Count);
        }
    }
}
