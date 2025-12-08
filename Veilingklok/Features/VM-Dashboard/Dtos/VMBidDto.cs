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
        var fullName = bid.Koper?.Gebruiker?.FullName;
        var username = bid.Koper?.Gebruiker?.Username;
        var fallbackNaam = bid.Koper?.Naam;

        var koperNaam =
            !string.IsNullOrWhiteSpace(fullName) ? fullName :
            !string.IsNullOrWhiteSpace(username) ? username :
            fallbackNaam ??
            string.Empty;

        return new VMBidDto
        {
            Id = bid.Id,
            VeilingProductId = bid.VeilingProductId,
            Amount = bid.Amount,
            KoperNaam = koperNaam,
            PlacedAtUtc = bid.PlacedAtUtc
        };
    }
}