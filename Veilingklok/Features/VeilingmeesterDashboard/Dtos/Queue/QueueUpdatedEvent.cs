using System.Collections.Generic;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Queue;

public sealed class QueueUpdatedEvent
{
    public int VeilingId { get; set; }
    public List<QueueItemDto> Queue { get; set; } = new();
}