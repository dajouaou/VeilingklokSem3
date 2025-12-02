using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Veiling.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services;

public sealed class VeilingService : IVeilingService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;

    public VeilingService(MyContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // ============================================================
    // CREATE
    // ============================================================
    public async Task<Result<VeilingDto>> CreateAsync(CreateVeilingDto dto)
    {
        var entity = _mapper.Map<Veiling>(dto);
        entity.Status = VeilingStatus.Draft;
        entity.CreatedAtUtc = DateTime.UtcNow;

        _db.Veilingen.Add(entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<VeilingDto>(entity));
    }

    // ============================================================
    // READ
    // ============================================================
    public async Task<Result<VeilingDto>> GetByIdAsync(int id)
    {
        var entity = await _db.Veilingen
            .AsNoTracking()
            .Include(v => v.VeilingProducten)
            .FirstOrDefaultAsync(v => v.Id == id);

        return entity is null
            ? Result.Fail<VeilingDto>("Veiling niet gevonden.")
            : Result.Success(_mapper.Map<VeilingDto>(entity));
    }

    public async Task<Result<List<VeilingDto>>> GetAllAsync()
    {
        var list = await _db.Veilingen
            .AsNoTracking()
            .ProjectTo<VeilingDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    // ============================================================
    // UPDATE
    // ============================================================
    public async Task<Result<VeilingDto>> UpdateAsync(int id, UpdateVeilingDto dto)
    {
        var entity = await _db.Veilingen.FindAsync(id);
        if (entity is null)
            return Result.Fail<VeilingDto>("Veiling niet gevonden.");

        _mapper.Map(dto, entity);

        await _db.SaveChangesAsync();
        return Result.Success(_mapper.Map<VeilingDto>(entity));
    }

    // ============================================================
    // DELETE
    // ============================================================
    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _db.Veilingen.FindAsync(id);
        if (entity is null)
            return Result.Fail<bool>("Veiling niet gevonden.");

        _db.Veilingen.Remove(entity);
        await _db.SaveChangesAsync();
        return Result.Success(true);
    }

    // ============================================================
    // START VEILING
    // ============================================================
    public async Task<Result<VeilingDto>> StartVeilingAsync(int id, StartVeilingDto dto)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.VeilingProducten)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (veiling is null)
            return Result.Fail<VeilingDto>("Veiling niet gevonden.");

        if (veiling.Status != VeilingStatus.Draft)
            return Result.Fail<VeilingDto>("Veiling kan alleen starten vanuit Draft.");

        veiling.Status = VeilingStatus.Running;
        veiling.StartTijdUtc = dto.StartTijdUtc;
        veiling.EindTijdUtc = dto.EindTijdUtc;

        veiling.VeilingProducten.Clear();

        int volgorde = 1;
        foreach (var pid in dto.VeilingProductIds)
        {
            veiling.VeilingProducten.Add(new VeilingProduct
            {
                VeilingId = veiling.Id,
                ProductId = pid,
                Volgorde = volgorde++
            });
        }

        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<VeilingDto>(veiling));
    }

    // ============================================================
    // ACTIEF LOT INSTELLEN
    // ============================================================
    public async Task<Result<VeilingDto>> SetCurrentLotAsync(int id, ChangeCurrentLotDto dto)
    {
        var veiling = await _db.Veilingen
            .Include(v => v.VeilingProducten)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (veiling is null)
            return Result.Fail<VeilingDto>("Veiling niet gevonden.");

        if (!veiling.VeilingProducten.Any(vp => vp.Id == dto.VeilingProductId))
            return Result.Fail<VeilingDto>("Lot zit niet in deze veiling.");

        veiling.CurrentVeilingProductId = dto.VeilingProductId;

        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<VeilingDto>(veiling));
    }

    // ============================================================
    // STOP VEILING
    // ============================================================
    public async Task<Result<VeilingDto>> StopVeilingAsync(int id)
    {
        var veiling = await _db.Veilingen.FindAsync(id);
        if (veiling is null)
            return Result.Fail<VeilingDto>("Veiling niet gevonden.");

        veiling.Status = VeilingStatus.Completed;
        veiling.EindTijdUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<VeilingDto>(veiling));
    }
}
