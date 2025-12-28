// Veilingklok/Features/VeilingmeesterDashboard/Dtos/PlanVeilingRequestDto.cs
using System.Collections.Generic;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public sealed class PlanVeilingRequestDto
{
    public string Leverdatum { get; set; } = string.Empty;
    public string Veildatum { get; set; } = string.Empty;
    public string StartTijd { get; set; } = string.Empty;
    public List<int> AanmeldingIds { get; set; } = new();
}