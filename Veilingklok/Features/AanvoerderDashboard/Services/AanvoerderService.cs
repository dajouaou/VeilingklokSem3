using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.AanvoerderDashboard.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.AanvoerderDashboard.Services;

public sealed class AanvoerderService : IAanvoerderService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;

    public AanvoerderService(MyContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<AanvoerderDto>> CreateAsync(CreateAanvoerderDto dto, int gebruikerId)
    {
        var user = await _db.Gebruikers.FirstOrDefaultAsync(u => u.Id == gebruikerId);
        if (user is null)
            return Result.Fail<AanvoerderDto>("Gebruiker bestaat niet.");

        if (user.Role != UserRole.Aanvoerder)
            return Result.Fail<AanvoerderDto>("Gebruiker heeft geen rol 'Aanvoerder'.");

        var bestaat = await _db.Aanvoerders.AnyAsync(a => a.GebruikerId == gebruikerId);
        if (bestaat)
            return Result.Fail<AanvoerderDto>("Aanvoerder-profiel bestaat al.");

        if (string.IsNullOrWhiteSpace(dto.Naam))
            return Result.Fail<AanvoerderDto>("Naam is verplicht.");

        var entity = new Aanvoerder
        {
            GebruikerId = gebruikerId,
            Naam = dto.Naam,
            ContactInfo = dto.ContactInfo
        };

        _db.Aanvoerders.Add(entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<AanvoerderDto>(entity));
    }

    public async Task<Result<AanvoerderDto>> GetByIdAsync(int id)
    {
        var dto = await _db.Aanvoerders
            .AsNoTracking()
            .Where(a => a.Id == id)
            .ProjectTo<AanvoerderDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return dto is null
            ? Result.Fail<AanvoerderDto>("Aanvoerder bestaat niet.")
            : Result.Success(dto);
    }

    public async Task<Result<List<AanvoerderDto>>> GetAllAsync()
    {
        var list = await _db.Aanvoerders
            .AsNoTracking()
            .OrderBy(a => a.Naam)
            .ProjectTo<AanvoerderDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    public async Task<Result<AanvoerderDto>> UpdateAsync(int id, UpdateAanvoerderDto dto)
    {
        var entity = await _db.Aanvoerders.FindAsync(id);
        if (entity is null)
            return Result.Fail<AanvoerderDto>("Aanvoerder niet gevonden.");

        if (string.IsNullOrWhiteSpace(dto.Naam))
            return Result.Fail<AanvoerderDto>("Naam is verplicht.");

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<AanvoerderDto>(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _db.Aanvoerders
            .Include(a => a.Producten)
            .Include(a => a.VeilingProducten)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (entity is null)
            return Result.Fail<bool>("Aanvoerder niet gevonden.");

        if (entity.Producten.Any())
            return Result.Fail<bool>("Kan niet verwijderen: aanvoerder heeft producten.");

        if (entity.VeilingProducten.Any())
            return Result.Fail<bool>("Kan niet verwijderen: aanvoerder heeft veilingproducten.");

        _db.Aanvoerders.Remove(entity);
        await _db.SaveChangesAsync();

        return Result.Success(true);
    }
}
