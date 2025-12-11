using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class VeilingPlanTests
    {
        [TestMethod]
        public async Task PlanVeiling_MaaktNieuweVeiling()
        {
            using var db = TestDbHelper.CreateInMemory();

            db.Aanmeldingen.Add(new Aanmelding { Id = 1, MinimumPrijs = 2.5m, Veildatum = DateTime.Today });
            db.SaveChanges();

            var controller = new VeilingPlanningController(db);

            var dto = new PlanVeilingRequestDto
            {
                Veildatum = DateTime.Today,
                StartTijd = "10:00",
                AanmeldingIds = new List<int> { 1 }
            };

            var result = await controller.PlanVeiling(dto);
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);
        }
    }
}
