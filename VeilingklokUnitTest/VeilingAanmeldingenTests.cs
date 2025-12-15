using Microsoft.VisualStudio.TestTools.UnitTesting;
using VeilingklokUnitTest;
using Veilingklok.Features.VeilingmeesterDashboard.Controllers;
using Veilingklok.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace VeilingklokUnitTest
{
    [TestClass]
    public class VeilingAanmeldingenTests
    {
        [TestMethod]
        public void GetAanmeldingenVoorDatum_GeeftItemsTerug()
        {
            using var db = TestDbHelper.CreateInMemory();

            var datum = DateTime.Today.Date;

            db.Aanvoerders.Add(new Aanvoerder { Id = 1, Naam = "Kees" });

            db.Aanmeldingen.Add(new Aanmelding
            {
                Id = 1,
                Soort = "Roos",
                Hoeveelheid = 10,
                MinimumPrijs = 2,
                AanvoerderId = 1,
                Veildatum = datum  
            });

            db.SaveChanges();

            var controller = new VeilingPlanningController(db);

            var result = controller.GetAanmeldingenVoorDatum(datum).Result as OkObjectResult;

            var list = result.Value as System.Collections.IEnumerable;
            int count = list.Cast<object>().Count();

            Assert.AreEqual(1, count);
        }
    }
}
