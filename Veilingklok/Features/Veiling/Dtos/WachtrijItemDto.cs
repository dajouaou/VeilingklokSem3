namespace Veilingklok.Features.Veiling.Dtos;

public class WachtrijItemDto
{
    public int VeilingProductId { get; set; }
    public int Volgorde { get; set; }
    public string Soort { get; set; } = "";
    public string? FotoUrl { get; set; }
    public decimal StartPrijs { get; set; }
    public int Hoeveelheid { get; set; }
}
