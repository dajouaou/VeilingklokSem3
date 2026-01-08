using Veilingklok.Core.Entities;

public class Transactie
{
    public int Id { get; set; }
    public int VeilingId { get; set; }
    public int VeilingProductId { get; set; }

    public int? KoperId { get; set; }
    public Gebruiker? Koper { get; set; }

    public int Aantal { get; set; }
    public decimal Prijs { get; set; }
    public DateTime Tijdstip { get; set; } = DateTime.UtcNow;

    public VeilingProduct VeilingProduct { get; set; } = null!;
}
