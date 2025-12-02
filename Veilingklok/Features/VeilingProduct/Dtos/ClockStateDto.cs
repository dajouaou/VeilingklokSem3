public sealed class ClockStateDto
{
    public int VeilingId { get; set; }
    public int VeilingProductId { get; set; }

    public int TimeLeftMs { get; set; }
    public bool IsRunning { get; set; }
}