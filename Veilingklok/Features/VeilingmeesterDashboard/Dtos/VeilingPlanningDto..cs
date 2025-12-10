namespace Veilingklok.Features.VeilingmeesterDashboard.Dtos
{
    /// <summary>
    /// DTO die jouw frontend gebruikt voor het overzicht van geplande veilingen.
    /// </summary>
    public class VeilingPlanningDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";          // frontend gebruikt v.naam
        public DateTime Veildatum { get; set; }         // frontend doet new Date(v.veildatum)
        public string StartTijd { get; set; } = "";     // "HH:mm"
        public string EindTijd { get; set; } = "";      // "HH:mm"
        public string Status { get; set; } = "Gepland"; // Gepland / Gestart / Afgesloten

        // Wordt gebruikt in jouw tabel: v.producten?.length
        public List<PlanProductDto> Producten { get; set; } = new();
    }
}
