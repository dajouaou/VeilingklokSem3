// Features/Veiling/Dtos/VeilingDto.cs
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class VeilingDto
{
    public int Id { get; set; }
    public int VeilingmeesterId { get; set; }

    public VeilingStatus Status { get; set; }
    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }
    public string? Locatie { get; set; }

    public int? CurrentVeilingProductId { get; set; }
    public List<int> VeilingProductIds { get; set; } = new();
}