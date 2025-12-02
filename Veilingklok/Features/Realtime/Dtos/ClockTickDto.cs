using System.Collections.Generic;
using Veilingklok.Features.VeilingProduct;

namespace Veilingklok.Features.Realtime.Dtos;

public sealed class ClockTickDto
{
    public int VeilingId { get; set; }
    public int VeilingProductId { get; set; }
    public int TimeLeftMs { get; set; }
    public decimal CurrentPrice { get; set; }
}

public sealed class QueueUpdatedDto
{
    public int VeilingId { get; set; }
    public List<VeilingProductDto> Queue { get; set; } = new();
}

public sealed class CurrentLotChangedDto
{
    public int VeilingId { get; set; }
    public int VeilingProductId { get; set; }
}

public sealed class VeilingStatusChangedDto
{
    public int VeilingId { get; set; }
    public string Status { get; set; } = string.Empty;
}