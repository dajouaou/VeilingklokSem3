using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Auction;

public sealed class StartAuctionRequestDto
{
    [Required]
    public DateTime StartTijdUtc { get; set; }

    [Required]
    public DateTime EindTijdUtc { get; set; }

    [MinLength(1)]
    public List<int> ProductIds { get; set; } = new();
}