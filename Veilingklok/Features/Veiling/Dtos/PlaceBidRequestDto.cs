// Features/Veiling/Dtos/PlaceBidRequestDto.cs
namespace Veilingklok.Features.Veiling.Dtos;

public sealed class PlaceBidRequestDto
{
    public int VeilingProductId { get; set; }
    public decimal Amount { get; set; }
}