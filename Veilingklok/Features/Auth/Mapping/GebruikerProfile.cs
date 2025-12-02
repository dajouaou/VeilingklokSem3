using AutoMapper;
using Veilingklok.Core.Entities;
using Veilingklok.Features.Auth.Dtos;

namespace Veilingklok.Features.Auth.Mapping;

public sealed class GebruikerProfile : Profile
{
    public GebruikerProfile()
    {
        // entity → dto voor /me endpoint
        CreateMap<Gebruiker, GebruikerDto>();
    }
}