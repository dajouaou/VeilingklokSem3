namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Queue;

public sealed class QueueGroupDto
{
    public string AanvoerderNaam { get; set; } = string.Empty;

    public List<QueueItemDto> Items { get; set; } = new();
}