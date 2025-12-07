// Veilingklok/Features/VM/Dtos/VMVeilingProductDto.cs
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMVeilingProductDto
{
    public int Id { get; set; }
    public int Volgorde { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string Aanvoerder { get; set; } = string.Empty;
    public VeilingProductStatus Status { get; set; }
    public decimal HuidigePrijs { get; set; }
    public int Hoeveelheid { get; set; }

    public static VMVeilingProductDto FromEntity(VeilingProduct p)
    {
        return new VMVeilingProductDto
        {
            Id = p.Id,
            Volgorde = p.Volgorde,
            ProductNaam = p.Product?.Naam ?? string.Empty,
            Aanvoerder = p.Aanvoerder?.Naam ?? string.Empty,
            Status = p.Status,
            HuidigePrijs = p.HuidigePrijs,
            Hoeveelheid = p.Hoeveelheid
        };
    }
}