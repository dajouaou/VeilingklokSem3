using Veilingklok.Core.Entities;

namespace Veilingklok.Core.Entities
{
    public class VeilingProduct
    {
        public int Id { get; set; }

        public int VeilingId { get; set; }
        public Veiling? Veiling { get; set; }

        public int AanmeldingId { get; set; }
        public Aanmelding? Aanmelding { get; set; }

        // Klokdata
        public decimal MaximumPrijs { get; set; }          // door veilingmeester
        public decimal MinimumPrijs { get; set; }          // uit aanmelding (kopie)
        public decimal HuidigePrijs { get; set; }          // live prijs

        public decimal DalingPerSeconde { get; set; }      // eur per seconde
        public int ResterendeHoeveelheid { get; set; }     // voor deelverkoop

        public bool IsActief { get; set; }
        public bool IsVerkocht { get; set; }
        public bool IsDoorgedraaid { get; set; }           // minimum bereikt zonder koper

        public int Volgorde { get; set; }

        public int? KoperId { get; set; }
        public Koper? Koper { get; set; }
        public DateTime? LaatstePrijsUpdateUtc { get; set; }


        public List<Bod> Biedingen { get; set; } = new();
        public List<Transactie> Transacties { get; set; } = new();

    }
}
