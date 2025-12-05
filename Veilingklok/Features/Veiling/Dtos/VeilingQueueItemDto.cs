using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class VeilingQueueItemDto
{
    public int Id { get; set; }
    public int Volgorde { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string Aanvoerder { get; set; } = string.Empty;

    public static VeilingQueueItemDto FromEntity(VeilingProduct p)
    {
        return new VeilingQueueItemDto
        {
            Id = p.Id,
            Volgorde = p.Volgorde,
            ProductNaam = p.Product?.Naam ?? "",
            Aanvoerder = p.Aanvoerder?.Naam ?? ""
        };
    }
}