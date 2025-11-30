using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class ReorderQueueDto
{
    [MinLength(1)]
    public List<ReorderQueueItemDto> Items { get; set; } = new();
}

public class ReorderQueueItemDto
{
    [Required]
    public int VeilingProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int NewPosition { get; set; }
}