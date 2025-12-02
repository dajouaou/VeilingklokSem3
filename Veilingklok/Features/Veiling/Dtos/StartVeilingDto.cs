// Features/Veiling/Dtos/StartVeilingDto.cs
namespace Veilingklok.Features.Veiling.Dtos;

public sealed class StartVeilingDto
{
    public DateTime StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }
    public List<int> VeilingProductIds { get; set; } = new();
}