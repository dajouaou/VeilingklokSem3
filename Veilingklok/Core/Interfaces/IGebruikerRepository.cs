namespace Veilingklok.Core.Interfaces {

    using Veilingklok.Core.Entities;

    // Interface voor alle database-acties rond gebruikers en hun rollen
    public interface IGebruikerRepository
    {
        // Zoekt een gebruiker op basis van email
        Task<Gebruiker?> GetByEmailAsync(string email);

        // Slaat een nieuwe gebruiker op
        Task AddAsync(Gebruiker gebruiker);

        // Maakt een koper-profiel aan voor een gebruiker
        Task CreateKoperAsync(Koper koper);

        // Maakt een aanvoerder-profiel aan voor een gebruiker
        Task CreateAanvoerderAsync(Aanvoerder aanvoerder);

        // Maakt een veilingmeester-profiel aan voor een gebruiker
        Task CreateVeilingmeesterAsync(Veilingmeester meester);
    }

}
