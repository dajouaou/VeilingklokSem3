using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Mapping;

public sealed class AuditEntryProfile : Profile
{
    public AuditEntryProfile()
    {
        CreateMap<AuditEntry, AuditEntryDto>();
        CreateMap<CreateAuditEntryDto, AuditEntry>();
    }
}