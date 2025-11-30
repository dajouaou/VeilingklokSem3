namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class VeilingDetailsDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "";
    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }

    public int? CurrentVeilingProductId { get; set; }
    public string? CurrentProductNaam { get; set; }
    public decimal? CurrentPrijs { get; set; }
}