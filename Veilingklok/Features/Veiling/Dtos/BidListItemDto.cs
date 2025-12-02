// Features/Veiling/Dtos/BidListItemDto.cs
namespace Veilingklok.Features.Veiling.Dtos;

public sealed class BidListItemDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Bidder { get; set; } = string.Empty;
    public DateTime PlacedAtUtc { get; set; }
}