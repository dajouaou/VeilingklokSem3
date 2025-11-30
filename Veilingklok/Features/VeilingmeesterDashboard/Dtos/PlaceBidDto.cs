using System.ComponentModel.DataAnnotations;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class PlaceBidDto
{
    [Required]
    public int VeilingProductId { get; set; }

    [Required]
    public int GebruikerId { get; set; }

    public int? KoperId { get; set; }

    [Range(0.01, 999999999)]
    public decimal Amount { get; set; }

    [Required]
    public BidSource Source { get; set; }
}