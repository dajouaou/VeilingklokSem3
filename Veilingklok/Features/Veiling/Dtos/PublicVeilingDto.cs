using System.Collections.Generic;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class PublicVeilingDto
{
    public PublicProductDto? CurrentProduct { get; set; }
    public List<PublicProductDto> Queue { get; set; } = new();
    public List<BodDto> Bids { get; set; } = new();
    public int TimeLeftMs { get; set; }
    public decimal CurrentPrice { get; set; }
    public int? HighestBidderId { get; set; }
}

