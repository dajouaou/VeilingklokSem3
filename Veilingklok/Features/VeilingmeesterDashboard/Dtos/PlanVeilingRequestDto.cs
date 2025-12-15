namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class PlanVeilingRequestDto
    {
        public string Leverdatum { get; set; } = "";
        public string Veildatum { get; set; } = "";
        public string StartTijd { get; set; } = "09:00";
        public List<int> AanmeldingIds { get; set; } = new();
    }
}
