namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Auction;

public sealed class VeilingStatusDto
{
    public int VeilingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }
}