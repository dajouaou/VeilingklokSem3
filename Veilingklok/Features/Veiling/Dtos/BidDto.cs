// Features/Veiling/Dtos/BidDto.cs
namespace Veilingklok.Features.Veiling.Dtos;

public sealed class BidDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime PlacedAtUtc { get; set; }
    public string Bidder { get; set; } = string.Empty;
}