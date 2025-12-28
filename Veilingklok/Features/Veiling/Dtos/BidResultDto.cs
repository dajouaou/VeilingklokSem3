namespace Veilingklok.Features.Veiling.Dtos;

public sealed class BidResultDto
{
    public bool Accepted { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal NewPrice { get; set; }
    public int? HighestBidderId { get; set; }
}