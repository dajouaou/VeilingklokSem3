using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Core.Interfaces
{
    // Interface die vastlegt welke veiling-acties de service moet ondersteunen
    public interface IVeilingService
    {
        // Haalt de actieve veiling op (gestart of gepauzeerd)
        Task<VeilingOverzichtDto?> GetActieveVeilingAsync();

        // Start een bestaande geplande veiling
        Task<VeilingOverzichtDto> StartGeplandeVeilingAsync(int veilingId);

        // Maakt en plant een nieuwe veiling op basis van aanmeldingen
        Task<VeilingOverzichtDto> StartVeilingAsync(DateTime veildatum, DateTime leverdatum, TimeSpan? startTijd = null);

        // Haalt alle details van één veiling op
        Task<VeilingOverzichtDto> GetDetailsAsync(int veilingId);

        // Zet een veiling op pauze
        Task PauseAsync(int veilingId);

        // Haalt een veiling van pauze en start hem weer
        Task ResumeAsync(int veilingId);

        // Stopt en sluit een veiling definitief af
        Task StopAsync(int veilingId);

        // Plaatst een bod op het huidige product in een veiling
        Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperGebruikerId);

        // Haalt alle veilingdagen op waar veilingen bestaan
        Task<List<string>> GetVeilingDagenAsync();
    }
}
