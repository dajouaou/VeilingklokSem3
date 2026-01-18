using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Core.Interfaces
{
    // Interface = contract: welke veiling-acties elke implementatie moet aanbieden
    public interface IVeilingService
    {
        // Actieve veiling = gestart of gepauzeerd. Kan null zijn als er niks actief is.
        Task<VeilingOverzichtDto?> GetActieveVeilingAsync();

        // Start een bestaande geplande veiling (id komt uit planning)
        Task<VeilingOverzichtDto> StartGeplandeVeilingAsync(int veilingId);

        // Start (en maakt) een veiling op basis van datums (wordt minder gebruikt in jouw flow)
        Task<VeilingOverzichtDto> StartVeilingAsync(DateTime veildatum, DateTime leverdatum, TimeSpan? startTijd = null);

        // Details van 1 veiling (incl huidig product + wachtrij etc)
        Task<VeilingOverzichtDto> GetDetailsAsync(int veilingId);

        // Statuswijzigingen
        Task PauseAsync(int veilingId);
        Task ResumeAsync(int veilingId);
        Task StopAsync(int veilingId);

        // Bod plaatsen: koperGebruikerId komt uit JWT/claims (auth)
        Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperGebruikerId);

        // Lever-/veildagen ophalen (voor planning in frontend)
        Task<List<string>> GetVeilingDagenAsync();
    }
}
