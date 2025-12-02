using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public sealed class VeilingDashboardReadService : IVeilingDashboardReadService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;

    public VeilingDashboardReadService(MyContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<VeilingDetailsDto?> GetDetailsAsync(int veilingId)
    {
        var v = await _db.Veilingen
            .AsNoTracking()
            .Include(x => x.CurrentVeilingProduct)!.ThenInclude(vp => vp!.Product)!.ThenInclude(p => p!.Aanvoerder)
            .FirstOrDefaultAsync(x => x.Id == veilingId);

        if (v is null)
            return null;

        var dto = _mapper.Map<VeilingDetailsDto>(v);

        dto.CurrentVeilingProductId = v.CurrentVeilingProductId;
        dto.CurrentProductNaam = v.CurrentVeilingProduct?.Product?.Naam;
        dto.CurrentPrijs = v.CurrentVeilingProduct?.HuidigePrijs;

        return dto;
    }

    public async Task<CurrentLotDto?> GetCurrentLotAsync(int veilingId)
    {
        var veiling = await _db.Veilingen
            .AsNoTracking()
            .Include(v => v.CurrentVeilingProduct)!.ThenInclude(vp => vp!.Product)!.ThenInclude(p => p!.Aanvoerder)
            .Include(v => v.CurrentVeilingProduct)!.ThenInclude(vp => vp!.Bids)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        var lot = veiling?.CurrentVeilingProduct;
        if (lot is null)
            return null;

        var dto = _mapper.Map<CurrentLotDto>(lot);

        dto.LastBid = lot.Bids.OrderByDescending(b => b.PlacedAtUtc).FirstOrDefault()?.Amount;
        dto.BidCount = lot.Bids.Count;

        return dto;
    }
}