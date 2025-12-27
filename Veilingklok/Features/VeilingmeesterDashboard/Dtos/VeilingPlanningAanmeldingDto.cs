// Veilingklok/Features/VeilingmeesterDashboard/Dtos/VeilingPlanningAanmeldingDto.cs
using System;

namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public sealed class VeilingPlanningAanmeldingDto
{
    public int Id { get; set; }
    public string Soort { get; set; } = string.Empty;
    public int Hoeveelheid { get; set; }
    public decimal MinimumPrijs { get; set; }
    public string AanvoerderNaam { get; set; } = string.Empty;
    public DateTime LeverDatum { get; set; }
}