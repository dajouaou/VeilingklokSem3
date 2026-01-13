using Veilingklok.Features.AanvoerderDashboard.Dtos;

namespace Veilingklok.Core.Interfaces
{
    public interface IAanvoerderDashboardService
    {
        // Maakt een nieuwe aanmelding aan voor de ingelogde aanvoerder
        Task<AanmeldingListItemDto> CreateAanmeldingAsync(int gebruikerId, AanmeldingCreateDto dto, string? fotoUrl);

        // Past een bestaande aanmelding aan (alleen van de ingelogde aanvoerder)
        Task<AanmeldingListItemDto> UpdateAanmeldingAsync(int gebruikerId, int id, AanmeldingUpdateDto dto, string? fotoUrl);

        // Verwijdert een aanmelding (alleen van de ingelogde aanvoerder)
        Task DeleteAanmeldingAsync(int gebruikerId, int id);

        // Haalt aanmeldingen op, eventueel gefilterd op veildatum
        Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(int gebruikerId, DateTime? veildatum);

        // Haalt dashboard statistieken op, eventueel gefilterd op veildatum
        Task<AanvoerderStatsDto> GetStatsAsync(int gebruikerId, DateTime? veildatum);
    }
}
