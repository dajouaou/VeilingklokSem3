using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    public sealed class VeilingPublicController_GetActief_ActieveVeiling_Tests
    {
        private static MyContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MyContext(opts);
        }

        [Fact]
        public async Task GetActief_StartedVeiling_ReturnsDto_WithHuidigProduct_AndWachtrij()
        {
            using var db = CreateDb();

            var aanvoerder = new Aanvoerder { Id = 10, Naam = "Jan" };
            db.Aanvoerders.Add(aanvoerder);

            var a1 = new Aanmelding
            {
                Id = 100,
                Soort = "Rozen",
                FotoUrl = "f1.jpg",
                MinimumPrijs = 1.10m,
                AanvoerderId = 10,
                Aanvoerder = aanvoerder,
                LeverDatum = DateTime.Today
            };
            var a2 = new Aanmelding
            {
                Id = 101,
                Soort = "Tulpen",
                FotoUrl = "f2.jpg",
                MinimumPrijs = 0.90m,
                AanvoerderId = 10,
                Aanvoerder = aanvoerder,
                LeverDatum = DateTime.Today
            };
            db.Aanmeldingen.AddRange(a1, a2);

            var v = new Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = new TimeSpan(9, 0, 0) };
            db.Veilingen.Add(v);

            var pActief = new VeilingProduct
            {
                Id = 200,
                VeilingId = 1,
                Veiling = v,
                AanmeldingId = 100,
                Aanmelding = a1,
                IsActief = true,
                IsVerkocht = false,
                IsDoorgedraaid = false,
                MaximumPrijs = 2.50m,
                MinimumPrijs = 1.10m,
                HuidigePrijs = 2.00m,
                DalingPerSeconde = 0.01m,
                ResterendeHoeveelheid = 100,
                Volgorde = 1
            };

            var pWacht = new VeilingProduct
            {
                Id = 201,
                VeilingId = 1,
                Veiling = v,
                AanmeldingId = 101,
                Aanmelding = a2,
                IsActief = false,
                IsVerkocht = false,
                IsDoorgedraaid = false,
                MaximumPrijs = 1.80m,
                MinimumPrijs = 0.90m,
                ResterendeHoeveelheid = 60,
                Volgorde = 2
            };

            v.HuidigProductId = 200;
            db.VeilingProducten.AddRange(pActief, pWacht);

            await db.SaveChangesAsync();

            var controller = new VeilingPublicController(db);

            var result = await controller.GetActief();

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

            Assert.Equal(1, dto.Id);
            Assert.True(dto.IsGestart);

            Assert.NotNull(dto.HuidigProduct);
            Assert.Equal(200, dto.HuidigProduct!.VeilingProductId);
            Assert.Equal("Rozen", dto.HuidigProduct.Soort);
            Assert.Equal("Jan", dto.HuidigProduct.AanvoerderNaam);

            Assert.Single(dto.Wachtrij);
            var w = dto.Wachtrij.Single();
            Assert.Equal(201, w.VeilingProductId);
            Assert.Equal("Tulpen", w.Soort);
        }

        [Fact]
        public async Task GetActief_HuidigProductIdNotFound_ReturnsDto_WithNullHuidigProduct()
        {
            using var db = CreateDb();

            var v = new Veiling { Id = 1, Status = VeilingStatus.Gestart, Datum = DateTime.Today, StartTijd = TimeSpan.Zero, HuidigProductId = 999 };
            db.Veilingen.Add(v);
            await db.SaveChangesAsync();

            var controller = new VeilingPublicController(db);

            var result = await controller.GetActief();

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);

            Assert.True(dto.IsGestart);
            Assert.Null(dto.HuidigProduct);
        }
    }
}
