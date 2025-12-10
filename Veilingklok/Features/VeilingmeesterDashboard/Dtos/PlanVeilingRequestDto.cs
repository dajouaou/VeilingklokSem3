namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class PlanVeilingRequestDto
    {
        public DateTime Veildatum { get; set; }   // datum-only
        public string StartTijd { get; set; } = "09:00"; // "HH:mm"
        public List<int> AanmeldingIds { get; set; } = new();
    }
}
