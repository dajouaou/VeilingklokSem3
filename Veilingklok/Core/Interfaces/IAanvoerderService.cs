using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.AanvoerderDashboard.Dtos;

namespace Veilingklok.Core.Interfaces
{
    /// <summary>
    /// Service-laag voor Aanvoerders (dashboard + beheer).
    /// Alle logica loopt via deze interface, controller blijft dun.
    /// </summary>
    public interface IAanvoerderService
    {
        // create: koppelt aanvoerder aan een bestaande gebruiker (gebruikerId uit JWT)
        Task<Result<AanvoerderDto>> CreateAsync(CreateAanvoerderDto dto, int gebruikerId);

        // read: één aanvoerder (incl. dashboard-info)
        Task<Result<AanvoerderDto>> GetByIdAsync(int id);

        // read: alle aanvoerders voor dashboard (met productcount/categorieën)
        Task<Result<List<AanvoerderDto>>> GetAllAsync();

        // update: naam/contactinfo aanpassen
        Task<Result<AanvoerderDto>> UpdateAsync(int id, UpdateAanvoerderDto dto);

        // delete: aanvoerder verwijderen (met business-checks in service)
        Task<Result<bool>> DeleteAsync(int id);
    }
}