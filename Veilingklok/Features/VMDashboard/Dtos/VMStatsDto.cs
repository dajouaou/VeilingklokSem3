namespace Veilingklok.Features.VM.Dtos;

public sealed class VMStatsDto
{
    public int TotaalProducten { get; set; }
    public int ProductenInQueue { get; set; }
    public int VerkochteProducten { get; set; }
    public int OvergeslagenProducten { get; set; }
    public int TotaalBiedingen { get; set; }
}
