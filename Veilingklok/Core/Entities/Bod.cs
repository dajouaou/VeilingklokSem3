namespace Veilingklok.Core.Entities
{
    public class Bod
    {
        public int Id { get; set; }

        public int VeilingId { get; set; }
        public Veiling Veiling { get; set; } = null!;   // ⭐ TOEVOEGEN

        public int VeilingProductId { get; set; }
        public VeilingProduct VeilingProduct { get; set; } = null!; // ⭐ al bijna goed

        public int KoperId { get; set; }
        public Koper? Koper { get; set; }

        public decimal Prijs { get; set; }
        public DateTime Tijdstip { get; set; } = DateTime.UtcNow;
    }
}
