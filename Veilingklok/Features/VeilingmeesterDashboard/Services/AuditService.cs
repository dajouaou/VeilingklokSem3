using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos.Audit;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public sealed class AuditService : IAuditService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;

    public AuditService(MyContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<AuditEntryDto>> LogAsync(CreateAuditEntryDto dto)
    {
        var entry = _mapper.Map<AuditEntry>(dto);
        entry.CreatedAtUtc = DateTime.UtcNow;

        _db.AuditEntries.Add(entry);
        await _db.SaveChangesAsync();

        return Result.Success(_mapper.Map<AuditEntryDto>(entry));
    }

    public async Task<Result<List<AuditEntryDto>>> GetByVeilingAsync(int veilingId)
    {
        var list = await _db.AuditEntries
            .AsNoTracking()
            .Where(a => a.VeilingId == veilingId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ProjectTo<AuditEntryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }

    public async Task<Result<List<AuditEntryDto>>> GetRecentAsync(int take = 50)
    {
        var list = await _db.AuditEntries
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAtUtc)
            .Take(take)
            .ProjectTo<AuditEntryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result.Success(list);
    }
}