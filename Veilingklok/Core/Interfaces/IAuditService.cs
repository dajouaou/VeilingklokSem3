using System.Collections.Generic;
using System.Threading.Tasks;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Audit;

namespace Veilingklok.Core.Interfaces;

public interface IAuditService
{
    Task<Result<AuditEntryDto>> LogAsync(CreateAuditEntryDto dto);
    Task<Result<List<AuditEntryDto>>> GetByVeilingAsync(int veilingId);
    Task<Result<List<AuditEntryDto>>> GetRecentAsync(int take = 50);
}