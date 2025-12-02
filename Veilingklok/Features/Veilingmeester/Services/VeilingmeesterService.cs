using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veilingmeester.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veilingmeester.Services;

public sealed class VeilingmeesterService : IVeilingmeesterService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;

    public VeilingmeesterService(MyContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // ============================================================
    // CREATE
    // ============================================================
    public async Task<Result<VeilingmeesterDto>> CreateAsync(CreateVeilingmeesterDto dto)
    {
        // Check of gekoppelde gebruiker bestaat
        var gebruiker = await _db.Gebruikers.FirstOrDefaultAsync(g => g.Id == dto.GebruikerId);
        if (gebruiker is null)
            return Result.Fail<VeilingmeesterDto>("Gebruiker bestaat niet.");

        // Check of gebruiker al een profiel heeft
        var exists = await _db.Veilingmeesters.AnyAsync(v => v.GebruikerId == dto.GebruikerId);
        if (exists)
            return Result.Fail<VeilingmeesterDto>("Deze gebruiker heeft al een veilingmeester-profiel.");

        var entity = _mapper.Map<Veilingmeester>(dto);

        _db.Veilingmeesters.Add(entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<VeilingmeesterDto>(entity));
    }

    // ============================================================
    // READ
    // ============================================================
    public async Task<Result<List<VeilingmeesterDto>>> GetAllAsync()
    {
        var list = await _db.Veilingmeesters
            .AsNoTracking()
            .Include(v => v.Veilingen)
            .ProjectTo<VeilingmeesterDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    public async Task<Result<VeilingmeesterDto>> GetByIdAsync(int id)
    {
        var entity = await _db.Veilingmeesters
            .AsNoTracking()
            .Include(v => v.Veilingen)
            .FirstOrDefaultAsync(v => v.Id == id);

        return entity is null
            ? Result.Fail<VeilingmeesterDto>("Veilingmeester niet gevonden.")
            : Result.Success(_mapper.Map<VeilingmeesterDto>(entity));
    }

    // ============================================================
    // UPDATE
    // ============================================================
    public async Task<Result<VeilingmeesterDto>> UpdateAsync(int id, UpdateVeilingmeesterDto dto)
    {
        var entity = await _db.Veilingmeesters.FirstOrDefaultAsync(v => v.Id == id);

        if (entity is null)
            return Result.Fail<VeilingmeesterDto>("Veilingmeester niet gevonden.");

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<VeilingmeesterDto>(entity));
    }

    // ============================================================
    // DELETE
    // ============================================================
    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _db.Veilingmeesters
            .Include(v => v.Veilingen)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (entity is null)
            return Result.Fail<bool>("Veilingmeester niet gevonden.");

        // Prevent deletion when active auctions exist
        if (entity.Veilingen.Any(v => v.Status == VeilingStatus.Running))
            return Result.Fail<bool>("Kan niet verwijderen: actieve veilingen gekoppeld.");

        _db.Veilingmeesters.Remove(entity);
        await _db.SaveChangesAsync();

        return Result.Success(true);
    }
}
