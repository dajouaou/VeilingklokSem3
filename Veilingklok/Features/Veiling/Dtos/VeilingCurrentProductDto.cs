using Veilingklok.Core.Entities;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class VeilingCurrentProductDto
{
    public int Id { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string Aanvoerder { get; set; } = string.Empty;
    public int Hoeveelheid { get; set; }
    public decimal HuidigePrijs { get; set; }

    public static VeilingCurrentProductDto FromEntity(VeilingProduct p)
    {
        return new VeilingCurrentProductDto
        {
            Id = p.Id,
            ProductNaam = p.Product?.Naam ?? "",
            FotoUrl = p.Product?.FotoUrl,
            Aanvoerder = p.Aanvoerder?.Naam ?? "",
            Hoeveelheid = p.Hoeveelheid,
            HuidigePrijs = p.HuidigePrijs
        };
    }
}