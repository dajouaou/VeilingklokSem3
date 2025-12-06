namespace Veilingklok.Infrastructure.Repositories 
{
    using Microsoft.EntityFrameworkCore;
    using Veilingklok.Core.Entities;
    using Veilingklok.Core.Interfaces;
    using Veilingklok.Infrastructure.Database;

    public class GebruikerRepository : IGebruikerRepository 
    {
        private readonly MyContext _context;

        public GebruikerRepository(MyContext context) 
        {
            _context = context;
        }

        public async Task<Gebruiker?> GetByEmailAsync(string email)
        {
            return await _context.Gebruikers
                .FirstOrDefaultAsync(g => g.Email == email);
        }

        public async Task AddAsync(Gebruiker gebruiker)
        {
            await _context.Gebruikers.AddAsync(gebruiker);
            await _context.SaveChangesAsync();
        }

      

        public async Task CreateKoperAsync(Koper koper)
        {
            await _context.Kopers.AddAsync(koper);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAanvoerderAsync(Aanvoerder aanvoerder)
        {
            await _context.Aanvoerders.AddAsync(aanvoerder);
            await _context.SaveChangesAsync();
        }

        public async Task CreateVeilingmeesterAsync(VM vm)
        {
            await _context.VMs.AddAsync(vm);
            await _context.SaveChangesAsync();
        }
    }
}