namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class VeilingDto
    {
        public int Id { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime? EindTijd { get; set; }
        public int? HuidigProductId { get; set; }
    }
}
