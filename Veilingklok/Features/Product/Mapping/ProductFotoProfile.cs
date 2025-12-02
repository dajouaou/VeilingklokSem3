using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Producten.Dtos;

public sealed class ProductFotoProfile : Profile
{
    public ProductFotoProfile()
    {
        CreateMap<ProductFoto, ProductFotoDto>();
        CreateMap<string, ProductFoto>()
            .ForMember(d => d.Url, opt => opt.MapFrom(src => src));
    }
}