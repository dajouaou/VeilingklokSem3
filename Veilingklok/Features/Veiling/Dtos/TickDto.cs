namespace Veilingklok.Features.Veiling.Dtos;

public sealed class TickDto
{
    public int VeilingId { get; set; }
    public int? VeilingProductId { get; set; }
    public decimal CurrentPrice { get; set; }
    public int TimeLeftMs { get; set; }
}