using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingProduct.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VeilingProduct.Services;

public sealed class VeilingProductService : IVeilingProductService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;

    public VeilingProductService(MyContext db, IMapper mapper, IAuditService audit)
    {
        _db = db;
        _mapper = mapper;
        _audit = audit;
    }

    // ============================================================
    // CREATE
    // ============================================================
    public async Task<Result<VeilingProductDto>> CreateAsync(CreateVeilingProductDto dto)
    {
        var product = await _db.Producten
            .Include(p => p.Fotos)
            .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

        if (product is null)
            return Result.Fail<VeilingProductDto>("Product niet gevonden.");

        var entity = _mapper.Map<VeilingProduct>(dto);

        // ============================
        // SNAPSHOTS (juridisch verplicht)
        // ============================
        entity.ProductNaamSnapshot = product.Naam;
        entity.CategorieSnapshot = product.Categorie;
        entity.KleurSnapshot = product.Kleur;
        entity.HoogteSnapshot = product.Hoogte;
        entity.AantalPerBosSnapshot = product.AantalPerBos;

        entity.FotoUrlSnapshot = product.Fotos.FirstOrDefault()?.Url;

        entity.AanvoerderId = product.AanvoerderId;
        entity.HuidigePrijs = dto.StartPrijs;
        entity.Status = VeilingProductStatus.InQueue;

        _db.VeilingProducten.Add(entity);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(new()
        {
            Action = "LOT_CREATED",
            VeilingId = entity.VeilingId,
            Details = $"Lot {entity.Id} toegevoegd voor Product {product.Naam}.",
            ActorGebruikerId = 0
        });

        return await GetByIdAsync(entity.Id);
    }

    // ============================================================
    // READ BY ID
    // ============================================================
    public async Task<Result<VeilingProductDto>> GetByIdAsync(int id)
    {
        var dto = await _db.VeilingProducten
            .AsNoTracking()
            .ProjectTo<VeilingProductDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(vp => vp.Id == id);

        return dto is null
            ? Result.Fail<VeilingProductDto>("Lot niet gevonden.")
            : Result.Success(dto);
    }

    // ============================================================
    // READ BY VEILING
    // ============================================================
    public async Task<Result<List<VeilingProductDto>>> GetByVeilingAsync(int veilingId)
    {
        var list = await _db.VeilingProducten
            .Where(vp => vp.VeilingId == veilingId)
            .OrderBy(vp => vp.Volgorde)
            .ProjectTo<VeilingProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    // ============================================================
    // UPDATE (mag niet tijdens Running of Sold)
    // ============================================================
    public async Task<Result<VeilingProductDto>> UpdateAsync(int id, UpdateVeilingProductDto dto)
    {
        var entity = await _db.VeilingProducten.FindAsync(id);
        if (entity is null)
            return Result.Fail<VeilingProductDto>("Lot niet gevonden.");

        if (entity.Status == VeilingProductStatus.Running)
            return Result.Fail<VeilingProductDto>("Kan lot niet wijzigen tijdens Running-status.");

        if (entity.Status == VeilingProductStatus.Sold)
            return Result.Fail<VeilingProductDto>("Kan verkocht lot niet wijzigen.");

        _mapper.Map(dto, entity);

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    // ============================================================
    // DELETE (alleen niet-running)
    // ============================================================
    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var e = await _db.VeilingProducten.FindAsync(id);
        if (e is null)
            return Result.Fail<bool>("Lot niet gevonden.");

        if (e.Status == VeilingProductStatus.Running)
            return Result.Fail<bool>("Kan actief lot niet verwijderen.");

        _db.VeilingProducten.Remove(e);
        await _db.SaveChangesAsync();

        return Result.Success(true);
    }

    // ============================================================
    // SET ACTIVE (start lot)
    // ============================================================
    public async Task<Result<VeilingProductDto>> SetActiveAsync(int id)
    {
        var e = await _db.VeilingProducten.FindAsync(id);
        if (e is null)
            return Result.Fail<VeilingProductDto>("Lot niet gevonden.");

        e.Status = VeilingProductStatus.Running;
        e.ActivatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(new()
        {
            Action = "LOT_STARTED",
            VeilingId = e.VeilingId,
            Details = $"Lot {e.Id} gestart.",
            ActorGebruikerId = 0
        });

        return await GetByIdAsync(id);
    }

    // ============================================================
    // MARK AS SOLD
    // ============================================================
    public async Task<Result<VeilingProductDto>> MarkAsSoldAsync(int id, int koperId)
    {
        var e = await _db.VeilingProducten.FindAsync(id);
        if (e is null)
            return Result.Fail<VeilingProductDto>("Lot niet gevonden.");

        e.Status = VeilingProductStatus.Sold;
        e.SoldToKoperId = koperId;
        e.ClosedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(new()
        {
            Action = "LOT_SOLD",
            VeilingId = e.VeilingId,
            Details = $"Lot {e.Id} verkocht aan koper {koperId}.",
            ActorGebruikerId = koperId
        });

        return await GetByIdAsync(id);
    }
}
