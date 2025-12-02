using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Bids;

public sealed class PlaceBidDto
{
    [Required]
    public int VeilingProductId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public BidSource Source { get; set; }

    // Wordt NIET uit body gehaald → uit JWT
    public int? KoperId { get; set; }
}