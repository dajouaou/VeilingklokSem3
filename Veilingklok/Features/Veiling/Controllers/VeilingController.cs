using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeilingController : ControllerBase
    {
        private readonly MyContext _context;

        public VeilingController(MyContext context)
        {
            _context = context;
        }

        // GET api/veiling?status=Actief
        [HttpGet]
        public async Task<IActionResult> GetByStatus([FromQuery] string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "Status is verplicht." });

            if (!Enum.TryParse<VeilingStatus>(status, true, out var parsedStatus))
                return BadRequest(new { message = "Ongeldige status." });

            var veilingen = await _context.Veilingen
                .Where(v => v.Status == parsedStatus)
                .OrderBy(v => v.Id)
                .ToListAsync();

            return Ok(veilingen);
        }
    }
}
