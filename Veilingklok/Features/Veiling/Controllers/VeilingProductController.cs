using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingProductController : ControllerBase
    {
        private readonly MyContext _ctx;

        public VeilingProductController(MyContext ctx)
        {
            _ctx = ctx;
        }

        // GET: api/veilingproduct/veiling/5
        [HttpGet("veiling/{veilingId}")]
        public async Task<IActionResult> GetByVeiling(int veilingId)
        {
            var koppelingen = await _ctx.VeilingProducten
                .Where(vp => vp.VeilingId == veilingId)
                .Include(vp => vp.Product)
                .OrderBy(vp => vp.Volgorde)
                .ToListAsync();

            return Ok(koppelingen);
        }
    }
}
