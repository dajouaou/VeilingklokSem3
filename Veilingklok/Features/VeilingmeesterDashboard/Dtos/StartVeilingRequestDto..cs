namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class StartVeilingRequestDto
    {
        public string Datum { get; set; } = "";
        public string StartTijd { get; set; } = "";
        public string EindTijd { get; set; } = "";
        public int ProductId { get; set; }
    }
}
