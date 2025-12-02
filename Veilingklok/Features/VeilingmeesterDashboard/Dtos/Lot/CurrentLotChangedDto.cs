namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Lot;

public sealed class CurrentLotChangedDto
{
    public int VeilingId { get; set; }
    public int VeilingProductId { get; set; }
    public string Status { get; set; } = string.Empty;
}