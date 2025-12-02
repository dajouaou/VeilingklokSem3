using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerderDashboard.Dtos;

public sealed class AanvoerderProfile : Profile
{
    public AanvoerderProfile()
    {
        // entity → dto (dashboard-ready)
        CreateMap<Aanvoerder, AanvoerderDto>()
            // aantal producten van deze aanvoerder
            .ForMember(d => d.ProductCount,
                o => o.MapFrom(s => s.Producten.Count))

            // unieke categorieën van zijn producten
            .ForMember(d => d.Categorieen,
                o => o.MapFrom(s =>
                    s.Producten
                        .Where(p => p.Categorie != null)
                        .Select(p => p.Categorie!)
                        .Distinct()
                ));

        // dto → entity (voor create)
        CreateMap<CreateAanvoerderDto, Aanvoerder>()
            // GebruikerId komt NIET uit dto maar uit JWT token → niet mappen
            .ForMember(d => d.GebruikerId, o => o.Ignore());

        // dto → entity (voor update)
        CreateMap<UpdateAanvoerderDto, Aanvoerder>();
    }
}