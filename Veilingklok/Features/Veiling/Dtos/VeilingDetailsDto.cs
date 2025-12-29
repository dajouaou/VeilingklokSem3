using System.Collections.Generic;
using Veilingklok.Core.Enums;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class VeilingDetailsDto
{
    public int Id { get; set; }
    public VeilingStatus Status { get; set; }
    public int? CurrentVeilingProductId { get; set; }
    public HuidigProductDto? CurrentProduct { get; set; }
    public List<WachtrijItemDto> Queue { get; set; } = new();
}
