using System.Threading.Tasks;
using Veilingklok.Core.Entities;

namespace Veilingklok.Core.Interfaces;

public interface IGebruikerRepository
{
    Task<Gebruiker?> GetByEmailAsync(string email);
    Task AddAsync(Gebruiker gebruiker);
    Task CreateKoperAsync(Koper koper);
    Task CreateAanvoerderAsync(Aanvoerder aanvoerder);
    Task CreateVeilingmeesterAsync(VM veilingmeester);
}