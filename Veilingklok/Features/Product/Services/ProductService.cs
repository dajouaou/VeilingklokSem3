using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Producten.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Producten.Services;

public sealed class ProductService : IProductService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly IUserContext _user;

    public ProductService(MyContext db, IMapper mapper, IUserContext user)
    {
        _db = db;
        _mapper = mapper;
        _user = user;
    }

    // ============================================================
    // CREATE
    // ============================================================
    public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        if (_user.Role is not (UserRole.Aanvoerder or UserRole.Admin))
            return Result.Fail<ProductDto>("Geen toegang.");

        if (_user.Role == UserRole.Aanvoerder && _user.UserId != dto.AanvoerderId)
            return Result.Fail<ProductDto>("Je mag alleen je eigen producten beheren.");

        var entity = _mapper.Map<Product>(dto);
        entity.CreatedAtUtc = DateTime.UtcNow;

        _db.Producten.Add(entity);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<ProductDto>(entity));
    }

    // ============================================================
    // GET DETAILS BY ID
    // ============================================================
    public async Task<Result<ProductDetailsDto>> GetByIdAsync(int id)
    {
        var entity = await _db.Producten
            .Where(p => !p.IsDeleted)
            .Include(p => p.Aanvoerder)
            .Include(p => p.Fotos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity is null)
            return Result.Fail<ProductDetailsDto>("Product niet gevonden.");

        return Result.Success(_mapper.Map<ProductDetailsDto>(entity));
    }

    // ============================================================
    // GET ALL (SUMMARIES)
    // ============================================================
    public async Task<Result<List<ProductSummaryDto>>> GetAllAsync()
    {
        var list = await _db.Producten
            .Where(p => !p.IsDeleted)
            .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    // ============================================================
    // GET BY AANVOERDER
    // ============================================================
    public async Task<Result<List<ProductSummaryDto>>> GetByAanvoerderAsync(int aanvoerderId)
    {
        if (_user.Role == UserRole.Aanvoerder && _user.UserId != aanvoerderId)
            return Result.Fail<List<ProductSummaryDto>>("Geen toegang.");

        var list = await _db.Producten
            .Where(p => p.AanvoerderId == aanvoerderId && !p.IsDeleted)
            .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    // ============================================================
    // SEARCH
    // ============================================================
    public async Task<Result<PagedResult<ProductSummaryDto>>> SearchAsync(SearchProductDto dto)
    {
        var query = _db.Producten
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Naam))
            query = query.Where(p => p.Naam.Contains(dto.Naam));

        if (!string.IsNullOrWhiteSpace(dto.Categorie))
            query = query.Where(p => p.Categorie == dto.Categorie);

        if (!string.IsNullOrWhiteSpace(dto.Kleur))
            query = query.Where(p => p.Kleur == dto.Kleur);

        if (dto.AanvoerderId is not null)
            query = query.Where(p => p.AanvoerderId == dto.AanvoerderId);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Naam)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(new PagedResult<ProductSummaryDto>(
            items, total, dto.Page, dto.PageSize));
    }

    // ============================================================
    // UPDATE
    // ============================================================
    public async Task<Result<ProductDto>> UpdateAsync(int id, UpdateProductDto dto)
    {
        var entity = await _db.Producten
            .Include(p => p.Fotos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity is null)
            return Result.Fail<ProductDto>("Product niet gevonden.");

        if (_user.Role == UserRole.Aanvoerder && _user.UserId != entity.AanvoerderId)
            return Result.Fail<ProductDto>("Geen toegang tot producten van andere aanvoerders.");

        if (!entity.RowVersion.SequenceEqual(dto.RowVersion))
            return Result.Fail<ProductDto>("Product is gewijzigd door een andere gebruiker. Ververs de pagina.");

        _mapper.Map(dto, entity);
        entity.UpdatedAtUtc = DateTime.UtcNow;

        entity.Fotos.Clear();
        if (dto.FotoUrls is not null)
        {
            foreach (var url in dto.FotoUrls)
                entity.Fotos.Add(new ProductFoto { Url = url });
        }

        await _db.SaveChangesAsync();
        return Result.Success(_mapper.Map<ProductDto>(entity));
    }

    // ============================================================
    // SOFT DELETE
    // ============================================================
    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _db.Producten.FirstOrDefaultAsync(p => p.Id == id);

        if (entity is null)
            return Result.Fail<bool>("Product niet gevonden.");

        if (_user.Role != UserRole.Admin)
            return Result.Fail<bool>("Alleen admin mag verwijderen.");

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Result.Success(true);
    }

    // ============================================================
    // RESTORE
    // ============================================================
    public async Task<Result<bool>> RestoreAsync(int id)
    {
        var entity = await _db.Producten.FirstOrDefaultAsync(p => p.Id == id);

        if (entity is null)
            return Result.Fail<bool>("Product niet gevonden.");

        if (_user.Role != UserRole.Admin)
            return Result.Fail<bool>("Alleen admin mag herstellen.");

        entity.IsDeleted = false;
        entity.DeletedAtUtc = null;

        await _db.SaveChangesAsync();
        return Result.Success(true);
    }
}
