using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Veiling.Mapping;

public sealed class BidProfile : Profile
{
    public BidProfile()
    {
        CreateMap<Bid, BidDto>()
            .ForMember(d => d.Bidder, opt => opt.MapFrom(src =>
                src.Koper != null ? src.Koper.Naam : "Systeem"));

        CreateMap<Bid, PublicBidDto>()
            .ForMember(d => d.Bidder, opt => opt.MapFrom(src =>
                src.Koper != null ? src.Koper.Naam : "Systeem"));

        CreateMap<Bid, BidListItemDto>()
            .ForMember(d => d.Bidder, opt => opt.MapFrom(src =>
                src.Koper != null ? src.Koper.Naam : "Systeem"));
    }
}