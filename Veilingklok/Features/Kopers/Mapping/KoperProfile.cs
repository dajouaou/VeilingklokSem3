using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Koper.Dtos;

namespace Veilingklok.Features.Koper.Mapping;

public sealed class KoperProfile : Profile
{
    public KoperProfile()
    {
        CreateMap<Koper, KoperDto>();
        CreateMap<Koper, KoperListItemDto>();
        CreateMap<CreateKoperDto, Koper>();
        CreateMap<UpdateKoperDto, Koper>().ForMember(x => x.Saldo, o => o.Ignore());
    }
}