// Veilingklok/Features/VeilingmeesterDashboard/Dtos/GeplandeVeilingListItemDto.cs
namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public sealed class GeplandeVeilingListItemDto
{
    public int Id { get; set; }
    public string Veildatum { get; set; } = string.Empty;
    public string StartTijd { get; set; } = string.Empty;
    public int AantalProducten { get; set; }
}