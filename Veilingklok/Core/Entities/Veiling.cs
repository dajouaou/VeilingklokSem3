using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities
{
    public class Veiling
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan StartTijd { get; set; }

        public DateTime? EindTijd { get; set; }

        public VeilingStatus Status { get; set; } = VeilingStatus.Gepland;

        public int? HuidigProductId { get; set; }
        public VeilingProduct? HuidigProduct { get; set; }

        public List<VeilingProduct> Producten { get; set; } = new();
        public List<Bod> Biedingen { get; set; } = new();
    }
}
