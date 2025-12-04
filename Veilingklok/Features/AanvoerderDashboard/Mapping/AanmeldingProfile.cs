using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerdersDashboard.Dtos;

namespace Veilingklok.Features.AanvoerdersDashboard.Mapping;

public class AanmeldingProfile : Profile
{
    public AanmeldingProfile()
    {
        CreateMap<Aanmelding, AanmeldingDto>()
            .ForMember(dest => dest.Locatie, opt => opt.MapFrom(src => src.Locatie.ToString()))
            .ForMember(dest => dest.VeilingDatum, opt => opt.MapFrom(src => src.VeilingDatum.ToString("yyyy-MM-dd")));

        CreateMap<NieuweAanmeldingDto, Aanmelding>();
    }
}
