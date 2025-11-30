using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Mapping;

public sealed class VeilingDashboardMappingProfile : Profile
{
    public VeilingDashboardMappingProfile()
    {
        CreateMap<Veiling, VeilingDetailsDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<VeilingProduct, CurrentLotDto>()
            .ForMember(d => d.VeilingProductId, o => o.MapFrom(s => s.Id))

            // product (expression-tree safe)
            .ForMember(d => d.ProductId,    o => o.MapFrom(s => s.ProductId))
            .ForMember(d => d.ProductNaam,  o => o.MapFrom(s => s.Product != null ? s.Product.Naam : ""))
            .ForMember(d => d.Categorie,    o => o.MapFrom(s => s.Product != null ? s.Product.Categorie : null))
            .ForMember(d => d.Beschrijving, o => o.MapFrom(s => s.Product != null ? s.Product.Beschrijving : null))
            .ForMember(d => d.FotoUrl,      o => o.MapFrom(s => s.Product != null ? s.Product.FotoUrl : null))

            // lot
            .ForMember(d => d.Hoeveelheid,  o => o.MapFrom(s => s.Hoeveelheid))
            .ForMember(d => d.StartPrijs,   o => o.MapFrom(s => s.StartPrijs))
            .ForMember(d => d.HuidigePrijs, o => o.MapFrom(s => s.HuidigePrijs))
            .ForMember(d => d.Status,       o => o.MapFrom(s => s.Status.ToString()))

            // aanvoerder (expression-tree safe)
            .ForMember(d => d.AanvoerderId, o => o.MapFrom(s => s.Product != null ? s.Product.AanvoerderId : 0))
            .ForMember(d => d.AanvoerderNaam, o => o.MapFrom(s =>
                s.Product != null
                    ? (s.Product.Aanvoerder != null ? s.Product.Aanvoerder.Naam : "")
                    : ""
            ))

            .ForMember(d => d.LastBid,  o => o.Ignore())
            .ForMember(d => d.BidCount, o => o.Ignore());

        CreateMap<VeilingProduct, QueueItemDto>()
            .ForMember(d => d.VeilingProductId, o => o.MapFrom(s => s.Id))

            // product (expression-tree safe)
            .ForMember(d => d.ProductId,   o => o.MapFrom(s => s.ProductId))
            .ForMember(d => d.ProductNaam, o => o.MapFrom(s => s.Product != null ? s.Product.Naam : ""))
            .ForMember(d => d.FotoUrl,     o => o.MapFrom(s => s.Product != null ? s.Product.FotoUrl : null))

            // lot
            .ForMember(d => d.Volgorde,    o => o.MapFrom(s => s.Volgorde))
            .ForMember(d => d.Hoeveelheid, o => o.MapFrom(s => s.Hoeveelheid))
            .ForMember(d => d.StartPrijs,  o => o.MapFrom(s => s.StartPrijs))
            .ForMember(d => d.Status,      o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Aanvoerder, QueueGroupDto>()
            .ForMember(d => d.AanvoerderId,   o => o.MapFrom(s => s.Id))
            .ForMember(d => d.AanvoerderNaam, o => o.MapFrom(s => s.Naam))
            .ForMember(d => d.Items,          o => o.Ignore());

        CreateMap<Bid, BidDto>()
            .ForMember(d => d.KoperNaam, o => o.MapFrom(s => s.Koper != null ? s.Koper.Naam : null))
            .ForMember(d => d.Source,    o => o.MapFrom(s => s.Source.ToString()));

        CreateMap<AuditEntry, AuditDto>()
            .ForMember(d => d.ActorNaam, o => o.MapFrom(s => s.ActorGebruiker != null ? s.ActorGebruiker.Username : ""));
    }
}
