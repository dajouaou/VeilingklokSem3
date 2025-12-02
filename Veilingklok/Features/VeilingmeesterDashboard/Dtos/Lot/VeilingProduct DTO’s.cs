namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos.Lot;

public sealed class VeilingProductDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string Naam { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public string? Categorie { get; set; }
    public string? Beschrijving { get; set; }

    public decimal StartPrijs { get; set; }
    public decimal? LaatsteBod { get; set; }
    public int? KoperId { get; set; }

    public int Volgorde { get; set; }
}