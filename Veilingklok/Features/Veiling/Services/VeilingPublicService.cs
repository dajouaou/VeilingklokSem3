using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Infrastructure.Database;
using BodEntity = Veilingklok.Core.Entities.Bod;

namespace Veilingklok.Features.VeilingPublic.Services
{
    public class VeilingPublicService : IVeilingPublicService
    {
        private readonly MyContext _db;

        public VeilingPublicService(MyContext db)
        {
            _db = db;
        }

        public async Task<List<string>> GetBeschikbareVeildagenAsync()
        {
            var dagen = await _db.Aanmeldingen
                .Select(a => a.Veildatum.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return dagen.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }
    }
}
