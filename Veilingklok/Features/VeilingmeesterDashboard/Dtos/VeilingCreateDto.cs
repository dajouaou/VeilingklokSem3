namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    /// <summary>
    /// DTO die binnenkomt vanuit je VeilingPlanning.jsx formulier.
    /// </summary>
    public class VeilingCreateDto
    {
        public string? Naam { get; set; }          // optioneel
        public DateTime Veildatum { get; set; }    // alleen datum (00:00)
        public string StartTijd { get; set; } = ""; // "HH:mm"
        public string EindTijd { get; set; } = "";  // "HH:mm"
        public List<int> ProductIds { get; set; } = new(); // Id’s van Aanmeldingen
    }
}
