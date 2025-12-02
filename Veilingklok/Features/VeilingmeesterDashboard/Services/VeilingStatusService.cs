using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Infrastructure.SignalR;
using Veilingklok.Infrastructure.SignalR.Events;

namespace Veilingklok.Features.VeilingmeesterDashboard.Services;

public sealed class VeilingStatusService : IVeilingStatusService
{
    private readonly MyContext _db;
    private readonly IAuctionEventDispatcher _events;

    public VeilingStatusService(MyContext db, IAuctionEventDispatcher events)
    {
        _db = db;
        _events = events;
    }

    public async Task<Result<int>> StartAuctionAsync(StartAuctionRequestDto dto)
    {
        if (dto.ProductIds == null || dto.ProductIds.Count == 0)
            return Result.Fail<int>("ProductIds mag niet leeg zijn.");

        if (dto.EindTijdUtc <= dto.StartTijdUtc)
            return Result.Fail<int>("Eindtijd moet na starttijd liggen.");

        var v = new Veiling
        {
            Status = VeilingStatus.Scheduled,
            StartTijdUtc = dto.StartTijdUtc,
            EindTijdUtc = dto.EindTijdUtc
        };

        _db.Veilingen.Add(v);
        await _db.SaveChangesAsync();

        var volgorde = 1;
        foreach (var productId in dto.ProductIds.Distinct())
        {
            _db.VeilingProducten.Add(new VeilingProduct
            {
                VeilingId = v.Id,
                ProductId = productId,
                Status = VeilingProductStatus.InQueue,
                StartPrijs = 0,
                HuidigePrijs = 0,
                Volgorde = volgorde++,
                Hoeveelheid = 1
            });
        }

        await _db.SaveChangesAsync();

        await _events.Publish(new AuctionStatusChangedEvent(v.Id, v.Status.ToString()));
        await _events.Publish(new QueueUpdatedEvent(v.Id));

        return Result.Success(v.Id);
    }

    public Task<Result> PauseAsync(int veilingId)
        => SetStatusAsync(veilingId, VeilingStatus.Paused);

    public Task<Result> ResumeAsync(int veilingId)
        => SetStatusAsync(veilingId, VeilingStatus.Running);

    public Task<Result> StopAsync(int veilingId)
        => SetStatusAsync(veilingId, VeilingStatus.Finished);

    private async Task<Result> SetStatusAsync(int veilingId, VeilingStatus status)
    {
        var v = await _db.Veilingen.FirstOrDefaultAsync(x => x.Id == veilingId);
        if (v is null)
            return Result.Fail("Veiling niet gevonden.");

        v.Status = status;
        await _db.SaveChangesAsync();

        await _events.Publish(new AuctionStatusChangedEvent(veilingId, status.ToString()));

        return Result.Success();
    }
}
