using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Veiling.Controllers;
using Veilingklok.Features.Veiling.Dtos;
using Xunit;

namespace VeilingklokUnitTest.Veiling
{
    // Unit tests voor het starten van een veiling en het ophalen van veilingdetails
    // Doel: controleren dat de controller correct samenwerkt met de service
    public sealed class VeilingController_Start_Tests
    {
        [Fact]
        public async Task Start_CallsService_AndReturnsOk()
        {
            // Arrange: mock van de veilingservice
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            // Input voor het starten van een veiling
            var veildatum = DateTime.Today.AddDays(7);
            var leverdatum = DateTime.Today.AddDays(8);

            var dto = new StartVeilingDto
            {
                Veildatum = veildatum,
                LeverDatum = leverdatum,
                StartTijd = new TimeSpan(9, 0, 0)
            };


            // Verwachte response van de service na succesvol starten
            var expected = new VeilingOverzichtDto
            {
                Id = 123,
                IsGestart = false
            };

            // Service wordt één keer aangeroepen met juiste parameters
            service
                .Setup(s => s.StartVeilingAsync(dto.Veildatum, dto.LeverDatum, dto.StartTijd))
                .ReturnsAsync(expected);

            // Controller maken met gemockte service
            var controller = new VeilingController(service.Object);

            // Act: start de veiling
            var result = await controller.Start(dto);

            // Assert: controller geeft Ok terug met exact het object van de service
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);

            service.VerifyAll();
        }

        [Fact]
        public async Task Start_ServiceThrows_Exception_IsPropagated()
        {
            // Arrange: service gooit een fout (bijv. ongeldige datum)
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            // Gebruik een datum die altijd in de toekomst ligt, anders returnt de controller BadRequest
            // door de tijdvalidatie (NlTime.Now()) en wordt de service nooit aangeroepen.
            var veildatum = DateTime.Today.AddDays(7);
            var leverdatum = DateTime.Today.AddDays(7); // bewust gelijk kan; service bepaalt of dit ongeldig is
            var startTijd = new TimeSpan(9, 0, 0);

            var dto = new StartVeilingDto
            {
                Veildatum = veildatum,
                LeverDatum = leverdatum,
                StartTijd = startTijd
            };

            service
                .Setup(s => s.StartVeilingAsync(dto.Veildatum, dto.LeverDatum, startTijd))
                .ThrowsAsync(new InvalidOperationException("Leverdatum ongeldig"));

            var controller = new VeilingController(service.Object);

            // Act + Assert: fout moet doorgegeven worden
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                controller.Start(dto));

            service.VerifyAll();
        }
        [Fact]
        public async Task GetDetails_ReturnsOk_FromService()
        {
            // Arrange: mock van de service
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            service
                .Setup(s => s.GetDetailsAsync(5))
                .ReturnsAsync(new VeilingOverzichtDto { Id = 5 });

            var controller = new VeilingController(service.Object);

            // Act: details van veiling ophalen
            var result = await controller.GetDetails(5);

            // Assert: controller geeft Ok terug met juiste veilinginformatie
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<VeilingOverzichtDto>(ok.Value);
            Assert.Equal(5, dto.Id);

            service.VerifyAll();
        }

        [Fact]
        public async Task GetDetails_ServiceThrows_Exception_IsPropagated()
        {
            // Arrange: service kan de veiling niet vinden
            var service = new Mock<IVeilingService>(MockBehavior.Strict);

            service
                .Setup(s => s.GetDetailsAsync(99))
                .ThrowsAsync(new KeyNotFoundException("Veiling niet gevonden"));

            var controller = new VeilingController(service.Object);

            // Act + Assert: fout moet doorgegeven worden
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                controller.GetDetails(99));

            service.VerifyAll();
        }
    }
}
