// Veilingklok/Features/VM/Dtos/VMQueueItemDto.cs
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMQueueItemDto
{
    public int Id { get; set; }
    public int Volgorde { get; set; }

    public int? ProductId { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }

    public int? AanvoerderId { get; set; }
    public string AanvoerderNaam { get; set; } = string.Empty;

    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public VeilingProductStatus Status { get; set; }

    public static VMQueueItemDto FromEntity(VeilingProduct p)
    {
        var productNaam =
            p.Product?.Naam
            ?? p.Aanmelding?.Soort
            ?? string.Empty;

        var fotoUrl =
            p.Product?.FotoUrl
            ?? p.Aanmelding?.FotoUrl;

        var aanvoerderNaam =
            p.Aanvoerder?.Naam
            ?? p.Aanmelding?.Aanvoerder?.Naam
            ?? string.Empty;

        return new VMQueueItemDto
        {
            Id = p.Id,
            Volgorde = p.Volgorde,
            ProductId = p.ProductId,
            ProductNaam = productNaam,
            FotoUrl = fotoUrl,
            AanvoerderId = p.AanvoerderId,
            AanvoerderNaam = aanvoerderNaam,
            Hoeveelheid = p.Hoeveelheid,
            StartPrijs = p.StartPrijs,
            HuidigePrijs = p.HuidigePrijs,
            Status = p.Status
        };
    }
}