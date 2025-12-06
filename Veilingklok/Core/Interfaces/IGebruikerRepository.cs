namespace Veilingklok.Core.Interfaces {

 using Veilingklok.Core.Entities;

    public interface IGebruikerRepository
{
    Task<Gebruiker?> GetByEmailAsync(string email);
    Task AddAsync(Gebruiker gebruiker);
    Task CreateKoperAsync(Koper koper);
    Task CreateAanvoerderAsync(Aanvoerder aanvoerder);
    Task CreateVeilingmeesterAsync(Veilingmeester meester);

    }

}
