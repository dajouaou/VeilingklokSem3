namespace Veilingklok.Core.Entities
{
    public class Bod
    {
        public int Id { get; set; }
        public int VeilingId { get; set; }
        public int VeilingProductId { get; set; }
        public int KoperId { get; set; }
        public decimal Prijs { get; set; }
        public DateTime Tijdstip { get; set; } = DateTime.UtcNow;

        public Koper? Koper { get; set; }
        public VeilingProduct? VeilingProduct { get; set; }
    }
}
