using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO met basisinformatie van een veiling voor publieke weergave
    public class VeilingPublicDto
    {
        public int Id { get; set; }
        // Uniek ID van de veiling

        public DateTime Veildatum { get; set; }
        // Datum waarop de veiling plaatsvindt

        public TimeSpan StartTijd { get; set; }
        // Starttijd van de veiling

        public DateTime? EindTijd { get; set; }
        // Eindtijd van de veiling (null zolang de veiling nog loopt)

        public VeilingStatus Status { get; set; }
        // Huidige status van de veiling (gepland, gestart, gepauzeerd, afgesloten)

        public int? HuidigProductId { get; set; }
        // ID van het product dat momenteel geveild wordt (null als er geen actief product is)
    }
}
