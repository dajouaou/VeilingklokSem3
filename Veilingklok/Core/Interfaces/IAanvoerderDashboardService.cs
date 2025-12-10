using Veilingklok.Features.AanvoerderDashboard.Dtos;

namespace Veilingklok.Core.Interfaces
{
    public interface IAanvoerderDashboardService
    {
        Task<AanmeldingListItemDto> CreateAanmeldingAsync(int gebruikerId, AanmeldingCreateDto dto, string? fotoUrl);
        Task<AanmeldingListItemDto> UpdateAanmeldingAsync(int gebruikerId, int id, AanmeldingUpdateDto dto, string? fotoUrl);
        Task DeleteAanmeldingAsync(int gebruikerId, int id);
        Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(int gebruikerId, DateTime? veildatum);
        Task<AanvoerderStatsDto> GetStatsAsync(int gebruikerId, DateTime? veildatum);
    }
}
