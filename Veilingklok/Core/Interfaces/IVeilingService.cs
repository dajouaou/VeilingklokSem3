using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Core.Interfaces
{
    public interface IVeilingService
    {
        Task<VeilingOverzichtDto?> GetActieveVeilingAsync();
        Task<VeilingOverzichtDto> StartGeplandeVeilingAsync(int veilingId);

        Task<VeilingOverzichtDto> StartVeilingAsync(DateTime veildatum, DateTime leverdatum, TimeSpan? startTijd = null);
        Task<VeilingOverzichtDto> GetDetailsAsync(int veilingId);

        Task PauseAsync(int veilingId);
        Task ResumeAsync(int veilingId);
        Task StopAsync(int veilingId);

        Task<BodDto> PlaatsBodAsync(int veilingId, BodPlaatsenDto dto, int koperGebruikerId);
        Task<List<string>> GetVeilingDagenAsync();
    }
}
