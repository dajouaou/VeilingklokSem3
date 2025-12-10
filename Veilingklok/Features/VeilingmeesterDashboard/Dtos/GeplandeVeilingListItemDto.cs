namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    public class GeplandeVeilingListItemDto
    {
        public int Id { get; set; }
        public string Veildatum { get; set; } = "";   // "yyyy-MM-dd"
        public string StartTijd { get; set; } = "";   // "HH:mm"
        public int AantalProducten { get; set; }
    }
}
