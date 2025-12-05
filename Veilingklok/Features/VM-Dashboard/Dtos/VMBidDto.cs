using Veilingklok.Core.Entities;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMBidDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string KoperNaam { get; set; } = string.Empty;
    public DateTime PlacedAtUtc { get; set; }

    public static VMBidDto FromEntity(Bid bid)
    {
        return new VMBidDto
        {
            Id = bid.Id,
            Amount = bid.Amount,
            KoperNaam = bid.Koper?.Naam ?? "",
            PlacedAtUtc = bid.PlacedAtUtc
        };
    }
}