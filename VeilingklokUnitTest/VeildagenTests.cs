using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class VeildagenTests
    {
        [TestMethod]
        public async Task GetVeildagen_GeeftUniekeDagen()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanmeldingen.Add(new Aanmelding { Veildatum = DateTime.Today });
            db.Aanmeldingen.Add(new Aanmelding { Veildatum = DateTime.Today.AddDays(1) });
            db.SaveChanges();

            var controller = new VeilingPlanningController(db);

            var result = await controller.GetVeildagen();
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);

            var dagen = ok.Value as IEnumerable<string>;
            Assert.AreEqual(2, dagen.Count());
        }
    }
}
