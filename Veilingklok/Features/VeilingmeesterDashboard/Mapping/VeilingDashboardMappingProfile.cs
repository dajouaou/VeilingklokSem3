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
                .ForMember(d => d.ResterendeHoeveelheid, m => m.MapFrom(s => s.ResterendeHoeveelheid))
                // Extra velden zodat AutoMapper niet klaagt
                .ForMember(d => d.VeilingProductId, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.AanvoerderId, m => m.MapFrom(s => s.Aanmelding!.AanvoerderId))
                .ForMember(d => d.AanvoerderNaam, m => m.MapFrom(s => s.Aanmelding!.Aanvoerder!.Naam));

            CreateMap<VeilingProduct, WachtrijItemDto>()
                .ForMember(d => d.Soort, m => m.MapFrom(s => s.Aanmelding!.Soort))
                .ForMember(d => d.FotoUrl, m => m.MapFrom(s => s.Aanmelding!.FotoUrl))
                .ForMember(d => d.ResterendeHoeveelheid, m => m.MapFrom(s => s.ResterendeHoeveelheid))
                // Extra velden zodat AutoMapper niet klaagt
                .ForMember(d => d.VeilingProductId, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.AanvoerderId, m => m.MapFrom(s => s.Aanmelding!.AanvoerderId))
                .ForMember(d => d.AanvoerderNaam, m => m.MapFrom(s => s.Aanmelding!.Aanvoerder!.Naam));
        }
    }
}
