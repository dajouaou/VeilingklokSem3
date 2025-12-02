using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Producten.Dtos;

public sealed class ProductReadModelProfile : Profile
{
    public ProductReadModelProfile()
    {
        // Summary
        CreateMap<Product, ProductSummaryDto>()
            .ForMember(d => d.FotoUrl,
                opt => opt.MapFrom(src => src.Fotos.FirstOrDefault().Url))
            .ForMember(d => d.AanvoerderNaam,
                opt => opt.MapFrom(src => src.Aanvoerder.Naam));

        // Details
        CreateMap<Product, ProductDetailsDto>()
            .ForMember(d => d.AanvoerderNaam,
                opt => opt.MapFrom(src => src.Aanvoerder.Naam))
            .ForMember(d => d.Fotos,
                opt => opt.MapFrom(src => src.Fotos));
    }
}