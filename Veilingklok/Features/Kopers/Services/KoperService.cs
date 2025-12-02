using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Kopers.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Kopers.Services;

public sealed class KoperService : IKoperService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;

    public KoperService(MyContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<KoperDto>> CreateByAdminAsync(CreateKoperDto dto)
    {
        var gebruiker = await _db.Gebruikers
            .FirstOrDefaultAsync(g => g.Role == UserRole.Koper);

        if (gebruiker is null)
            return Result.Fail<KoperDto>("Geen gebruiker met rol 'Koper' beschikbaar.");

        var bestaat = await _db.Kopers.AnyAsync(x => x.GebruikerId == gebruiker.Id);
        if (bestaat)
            return Result.Fail<KoperDto>("Koperprofiel bestaat al.");

        var entity = new Koper
        {
            GebruikerId = gebruiker.Id,
            Naam = dto.Naam,
            Saldo = 0m
        };

        _db.Kopers.Add(entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<KoperDto>(entity));
    }

    public async Task<Result<KoperDto>> GetByUserIdAsync(int gebruikerId)
    {
        var entity = await _db.Kopers
            .AsNoTracking()
            .Include(k => k.Bids)
            .Include(k => k.GekochteVeilingProducten)
            .FirstOrDefaultAsync(k => k.GebruikerId == gebruikerId);

        return entity is null
            ? Result.Fail<KoperDto>("Koperprofiel niet gevonden.")
            : Result.Success(_mapper.Map<KoperDto>(entity));
    }

    public async Task<Result<List<KoperListItemDto>>> GetAllAsync()
    {
        var list = await _db.Kopers
            .AsNoTracking()
            .ProjectTo<KoperListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    public async Task<Result<KoperDto>> UpdateByUserIdAsync(int gebruikerId, UpdateKoperDto dto)
    {
        var entity = await _db.Kopers.FirstOrDefaultAsync(k => k.GebruikerId == gebruikerId);
        if (entity is null)
            return Result.Fail<KoperDto>("Koperprofiel niet gevonden.");

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<KoperDto>(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _db.Kopers.FirstOrDefaultAsync(k => k.Id == id);
        if (entity is null)
            return Result.Fail<bool>("Koper niet gevonden.");

        var heeftHistorie = await _db.Bids.AnyAsync(b => b.KoperId == id);
        if (heeftHistorie)
            return Result.Fail<bool>("Koper kan niet worden verwijderd vanwege biedhistorie.");

        _db.Kopers.Remove(entity);
        await _db.SaveChangesAsync();

        return Result.Success(true);
    }
}
