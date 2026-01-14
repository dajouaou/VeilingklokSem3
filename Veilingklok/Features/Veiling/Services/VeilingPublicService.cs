using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VeilingPublic.Services
{
    // Service die openbare veiling-info ophaalt voor de public pagina’s
    public class VeilingPublicService : IVeilingPublicService
    {
        private readonly MyContext _db;

        // Injecteert de database context
        public VeilingPublicService(MyContext db)
        {
            _db = db;
        }

        // Haalt alle beschikbare veildagen op en zet ze om naar strings
        public async Task<List<string>> GetBeschikbareVeildagenAsync()
        {
            var dagen = await _db.Aanmeldingen
                .Select(a => a.LeverDatum.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }
    }
}
