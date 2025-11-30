namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos;

public class QueueItemDto
{
    public int VeilingProductId { get; set; }

    public int ProductId { get; set; }
    public string ProductNaam { get; set; } = "";
    public string? FotoUrl { get; set; }

    public int Volgorde { get; set; }
    public int Hoeveelheid { get; set; }
    public decimal StartPrijs { get; set; }

    public string Status { get; set; } = "";
} 