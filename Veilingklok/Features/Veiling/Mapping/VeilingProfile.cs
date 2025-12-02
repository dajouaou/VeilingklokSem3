using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Mapping;

public sealed class VeilingProfile : Profile
{
    public VeilingProfile()
    {
        CreateMap<Veiling, VeilingDto>()
            .ForMember(d => d.VeilingProductIds,
                opt => opt.MapFrom(src => src.VeilingProducten.Select(vp => vp.Id)));

        CreateMap<CreateVeilingDto, Veiling>();
        CreateMap<UpdateVeilingDto, Veiling>();
    }
}