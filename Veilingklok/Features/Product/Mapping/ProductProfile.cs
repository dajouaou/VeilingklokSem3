using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Producten.Dtos;

namespace Veilingklok.Features.Producten.Mapping;

public sealed class ProductProfile : Profile
{
    public ProductProfile()
    {
        // READ → Entity → DTO
        CreateMap<Product, ProductDto>();

        // CREATE → DTO → Entity
        CreateMap<CreateProductDto, Product>()
            .ForMember(d => d.Fotos,
                opt => opt.MapFrom(src => src.FotoUrls));

        // UPDATE → DTO → Entity
        CreateMap<UpdateProductDto, Product>()
            .ForMember(d => d.Fotos,
                opt => opt.MapFrom(src => src.FotoUrls))
            .ForMember(d => d.RowVersion,
                opt => opt.Ignore()); // handled manually
    }
}