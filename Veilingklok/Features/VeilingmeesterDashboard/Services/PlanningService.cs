using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Interfaces;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services
{
    public class PlanningService : IPlanningService
    {
        private readonly MyContext _db;

        public PlanningService(MyContext db)
        {
            _db = db;
        }

        // ✔ Haal alle producten op die nog niet in een veiling zitten
        public async Task<List<PlanProductDto>> GetBeschikbareProductenAsync()
        {
            return await _db.Aanmeldingen
                .Where(a => a.VeilingProductId == null) // nog niet gekoppeld aan een veilingproduct
                .Select(a => new PlanProductDto
                {
                    Id = a.Id,
                    Soort = a.Soort,
                    Hoeveelheid = a.Hoeveelheid,
                    AanvoerderNaam = a.Aanvoerder.Naam,
                    FotoUrl = a.FotoUrl
                })
                .ToListAsync();
        }

        // ✔ Start een veiling met het gekozen product
        public async Task<VeilingDto> StartVeilingMetPlanningAsync(StartVeilingRequestDto dto)
        {
            var aanmelding = await _db.Aanmeldingen
                .FirstOrDefaultAsync(a => a.Id == dto.ProductId);

            if (aanmelding == null)
                throw new Exception("Aanmelding niet gevonden");

            // Maak een nieuw veilingproduct aan
            var veilingProduct = new VeilingProduct
            {
                AanmeldingId = aanmelding.Id,
                Aanmelding = aanmelding,
                StartPrijs = aanmelding.MinimumPrijs,
                HuidigePrijs = aanmelding.MinimumPrijs,
                IsActief = false,
                IsVerkocht = false,
                Volgorde = 1
            };

            // Combineer datum + start/eindtijd
            var start = DateTime.Parse($"{dto.Datum} {dto.StartTijd}");
            var eind = DateTime.Parse($"{dto.Datum} {dto.EindTijd}");

            // LET OP → volledige naam gebruiken ivm namespace-conflict!
            var veiling = new Veilingklok.Core.Entities.Veiling
            {
                StartTijd = start,
                EindTijd = eind,
                IsGestart = false,
                IsPauze = false,
                IsAfgesloten = false,
                HuidigProduct = veilingProduct,
                Producten = new List<VeilingProduct> { veilingProduct }
            };

            _db.Veilingen.Add(veiling);
            await _db.SaveChangesAsync();

            return new VeilingDto
            {
                Id = veiling.Id,
                StartTijd = veiling.StartTijd,
                EindTijd = veiling.EindTijd,
                HuidigProductId = veiling.HuidigProductId
            };
        }
    }
}
