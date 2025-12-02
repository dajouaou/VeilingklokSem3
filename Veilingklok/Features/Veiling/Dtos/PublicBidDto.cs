// Features/Veiling/Dtos/PublicBidDto.cs
namespace Veilingklok.Features.Veiling.Dtos;

public sealed class PublicBidDto
{
    public decimal Amount { get; set; }
    public string Bidder { get; set; } = string.Empty;
    public DateTime PlacedAtUtc { get; set; }
}