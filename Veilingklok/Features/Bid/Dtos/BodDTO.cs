namespace Veilingklok.Features.Bod.Dtos
{
    public class BodDTO
    {
        public int Id { get; set; }
        public decimal Bedrag { get; set; }
        public DateTime Tijdstip { get; set; }
        public string KoperUsername { get; set; } = string.Empty;
        public int VeilingId { get; set; }
    }
}
