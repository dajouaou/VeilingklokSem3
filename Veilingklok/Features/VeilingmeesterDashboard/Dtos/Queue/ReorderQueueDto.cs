using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Queue;

public sealed class ReorderQueueDto
{
    [Required]
    public List<ReorderQueueItem> Items { get; set; } = new();
}

public sealed class ReorderQueueItem
{
    [Required]
    public int VeilingProductId { get; set; }

    [Required]
    public int NewPosition { get; set; }
}