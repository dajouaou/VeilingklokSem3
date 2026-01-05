namespace Veilingklok.Core.Entities
{
    public class Bod
    {
        public int Id { get; set; }

        public int VeilingId { get; set; }
        public Veiling Veiling { get; set; } = null!;

        public int VeilingProductId { get; set; }
        public VeilingProduct VeilingProduct { get; set; } = null!;

        public int KoperId { get; set; }
        public Koper? Koper { get; set; }

        public int Aantal { get; set; }                    // ✅ nodig voor deelverkoop
        public decimal Prijs { get; set; }
        public DateTime Tijdstip { get; set; } = DateTime.UtcNow;
    }
}
