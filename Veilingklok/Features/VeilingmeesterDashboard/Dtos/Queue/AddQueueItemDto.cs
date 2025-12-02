using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Queue;

public sealed class AddQueueItemDto
{
    [Required]
    public int ProductId { get; set; }

    // 0 = auto-append, service bepaalt juiste volgorde
    public int Volgorde { get; set; } = 0;

    [Range(1, 999999)]
    public int Hoeveelheid { get; set; } = 1;

    [Range(0, 999999999)]
    public decimal StartPrijs { get; set; }
}