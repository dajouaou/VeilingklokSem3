namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Clock;

public sealed class ClockTickEvent
{
    public ClockStateDto Clock { get; set; } = new();
}