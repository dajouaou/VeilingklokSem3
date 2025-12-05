using Veilingklok.Core.Entities;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMCurrentProductDto
{
    public int Id { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string Aanvoerder { get; set; } = string.Empty;
    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public int Hoeveelheid { get; set; }
    public List<VMBidDto> Bids { get; set; } = new();

    public static VMCurrentProductDto FromEntity(VeilingProduct p)
    {
        return new VMCurrentProductDto
        {
            Id = p.Id,
            ProductNaam = p.Product?.Naam ?? "",
            Aanvoerder = p.Aanvoerder?.Naam ?? "",
            StartPrijs = p.StartPrijs,
            HuidigePrijs = p.HuidigePrijs,
            Hoeveelheid = p.Hoeveelheid,
            Bids = p.Bids.Select(VMBidDto.FromEntity).ToList()
        };
    }
}