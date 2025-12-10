// VeilingProduct.cs

using Veilingklok.Core.Entities;

public class VeilingProduct
{
    public int Id { get; set; }

    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    public int AanmeldingId { get; set; }
    public Aanmelding? Aanmelding { get; set; }
    public decimal StartPrijs { get; set; }
    public decimal HuidigePrijs { get; set; }
    public bool IsActief { get; set; } 

    public bool IsVerkocht { get; set; }

    public int Volgorde { get; set; }

    public int? KoperId { get; set; }
    public Koper? Koper { get; set; }

    public List<Bod> Biedingen { get; set; } = new();
}
