using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.VeilingProduct.Dtos;

public sealed class VeilingProductProfile : Profile
{
    public VeilingProductProfile()
    {
        CreateMap<VeilingProduct, VeilingProductDto>()
            .ForMember(d => d.ProductNaam, o => o.MapFrom(s => s.ProductNaamSnapshot))
            .ForMember(d => d.FotoUrl, o => o.MapFrom(s => s.FotoUrlSnapshot))
            .ForMember(d => d.Categorie, o => o.MapFrom(s => s.CategorieSnapshot))
            .ForMember(d => d.Kleur, o => o.MapFrom(s => s.KleurSnapshot))
            .ForMember(d => d.Hoogte, o => o.MapFrom(s => s.HoogteSnapshot))
            .ForMember(d => d.AantalPerBos, o => o.MapFrom(s => s.AantalPerBosSnapshot))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateVeilingProductDto, VeilingProduct>();
        CreateMap<UpdateVeilingProductDto, VeilingProduct>();
    }
}