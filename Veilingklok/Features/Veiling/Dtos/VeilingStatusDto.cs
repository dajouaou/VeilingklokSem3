using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class VeilingStatusDto
{
    public int VeilingId { get; set; }
    public VeilingStatus Status { get; set; }
    public int? CurrentVeilingProductId { get; set; }
}