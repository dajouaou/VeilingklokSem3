

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class CurrentLotDto
{
    public int VeilingProductId { get; set; }
    public string ProductNaam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public int Aantal { get; set; }
    public decimal HuidigePrijs { get; set; }
    public string AanvoerderNaam { get; set; } = string.Empty;
    public VeilingProductStatus Status { get; set; }
}
