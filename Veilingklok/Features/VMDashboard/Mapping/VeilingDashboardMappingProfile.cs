using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Mapping
{
    public class VeilingDashboardMappingProfile : Profile
    {
        public VeilingDashboardMappingProfile()
        {
            CreateMap<VeilingProduct, HuidigProductDto>()
                .ForMember(d => d.Soort, m => m.MapFrom(s => s.Aanmelding!.Soort))
                .ForMember(d => d.FotoUrl, m => m.MapFrom(s => s.Aanmelding!.FotoUrl))
                .ForMember(d => d.Hoeveelheid, m => m.MapFrom(s => s.Aanmelding!.Hoeveelheid));

            CreateMap<VeilingProduct, WachtrijItemDto>()
                .ForMember(d => d.Soort, m => m.MapFrom(s => s.Aanmelding!.Soort))
                .ForMember(d => d.FotoUrl, m => m.MapFrom(s => s.Aanmelding!.FotoUrl))
                .ForMember(d => d.Hoeveelheid, m => m.MapFrom(s => s.Aanmelding!.Hoeveelheid));
        }
    }
}
