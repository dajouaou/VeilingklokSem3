namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class QueueGroupDto
{
    public int AanvoerderId { get; set; }
    public string AanvoerderNaam { get; set; } = "";

    public List<QueueItemDto> Items { get; set; } = new();
}