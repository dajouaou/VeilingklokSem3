using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VeilingPublic.Services
{
    // Service voor publieke veilinginformatie
    public class VeilingPublicService : IVeilingPublicService
    {
        private readonly MyContext _db;

        public VeilingPublicService(MyContext db)
        {
            _db = db; // DbContext via dependency injection
        }

        // Haalt alle beschikbare veildagen op
        public async Task<List<string>> GetBeschikbareVeildagenAsync()
        {
            var dagen = await _db.Aanmeldingen
                .Select(a => a.LeverDatum.Date) // alleen datum, geen tijd
                .Distinct()                     // dubbele dagen verwijderen
                .OrderBy(d => d)                // sorteren op datum
                .ToListAsync();

            return dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            // Datum formatteren voor frontend
        }
    }
}
