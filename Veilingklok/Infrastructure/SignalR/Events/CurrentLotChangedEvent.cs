using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Infrastructure.SignalR.Events;

public record CurrentLotChangedEvent(int VeilingId, CurrentLotDto Lot);