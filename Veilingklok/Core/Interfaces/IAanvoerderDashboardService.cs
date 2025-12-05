using Veilingklok.Features.AanvoerderDashboard.Dtos;

namespace Veilingklok.Core.Interfaces
{
    public interface IAanvoerderDashboardService
    {
        Task<AanmeldingListItemDto> CreateAanmeldingAsync(int gebruikerId, AanmeldingCreateDto dto);
        Task<IReadOnlyList<AanmeldingListItemDto>> GetAanmeldingenAsync(int gebruikerId, DateTime? veildatum);
        Task<AanvoerderStatsDto> GetStatsAsync(int gebruikerId, DateTime? veildatum);
    }
}
