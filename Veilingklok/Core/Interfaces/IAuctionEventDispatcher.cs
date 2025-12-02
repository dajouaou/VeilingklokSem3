using Veilingklok.Features.Realtime.Dtos;

public interface IAuctionEventDispatcher
{
    Task ClockTickAsync(ClockTickDto dto);
    Task QueueUpdatedAsync(QueueUpdatedDto dto);
    Task CurrentLotChangedAsync(CurrentLotChangedDto dto);
    Task VeilingStatusChangedAsync(VeilingStatusChangedDto dto);
}