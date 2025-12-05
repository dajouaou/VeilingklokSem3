namespace Veilingklok.Core.Entities
{
    public class Veiling
    {
        public int Id { get; set; }

        public DateTime StartTijd { get; set; }
        public DateTime? EindTijd { get; set; }

        public bool IsGestart { get; set; }
        public bool IsPauze { get; set; }
        public bool IsAfgesloten { get; set; }

        public int? HuidigProductId { get; set; }
        public VeilingProduct? HuidigProduct { get; set; }

        public List<VeilingProduct> Producten { get; set; } = new();
        public List<Bod> Biedingen { get; set; } = new();
    }
}
