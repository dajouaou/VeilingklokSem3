

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class ClockStateDto
{
    public int VeilingId { get; set; }
    public int? VeilingProductId { get; set; }
    public long TimeLeftMs { get; set; }
    public bool IsRunning { get; set; }
}
