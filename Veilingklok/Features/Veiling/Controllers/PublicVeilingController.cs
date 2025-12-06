using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Route("api/veiling-public")]
    public class VeilingPublicController : ControllerBase
    {
        private readonly MyContext _db;

        public VeilingPublicController(MyContext db)
        {
            _db = db;
        }

        [HttpGet("dagen")]
        public async Task<ActionResult<List<string>>> GetPublicVeildagen()
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
