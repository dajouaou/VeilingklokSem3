using Veilingklok.Features.Veiling.Dtos;

namespace Veilingklok.Features.Koper.Dtos;

public sealed class KoperDto
{
    public int Id { get; set; }
    public int GebruikerId { get; set; }
    public string Naam { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public List<BidDto> Bids { get; set; } = new();
}