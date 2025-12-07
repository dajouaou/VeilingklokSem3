// Veilingklok/Features/VM/Dtos/VMBidDto.cs
using System;
using Veilingklok.Core.Entities;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMBidDto
{
    public int Id { get; set; }
    public int VeilingProductId { get; set; }
    public decimal Amount { get; set; }
    public string KoperNaam { get; set; } = string.Empty;
    public DateTime PlacedAtUtc { get; set; }

    public static VMBidDto FromEntity(Bid bid)
    {
        return new VMBidDto
        {
            Id = bid.Id,
            VeilingProductId = bid.VeilingProductId,
            Amount = bid.Amount,
            KoperNaam = bid.Koper?.Naam ?? string.Empty,
            PlacedAtUtc = bid.PlacedAtUtc
        };
    }
}