using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Infrastructure.Database;


namespace Veilingklok.Features.AanvoerdersDashboard.Services;

public class AanmeldingService
{
    private readonly MyContext _context;

    public AanmeldingService(MyContext context)
    {
        _context = context;
    }

    public async Task<List<Aanmelding>> GetAanmeldingenAsync()
    {
        return await _context.Aanmeldingen
            .OrderBy(a => a.VeilingDatum)
            .ToListAsync();
    }

    public async Task<Aanmelding> CreateAanmeldingAsync(Aanmelding nieuwe)
    {
        _context.Aanmeldingen.Add(nieuwe);
        await _context.SaveChangesAsync();
        return nieuwe;
    }
}
