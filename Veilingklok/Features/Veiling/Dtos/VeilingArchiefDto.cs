namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO voor een afgeronde veiling in het archief
    public class VeilingArchiefDto
    {
        public int Id { get; set; }
        // Uniek ID van de veiling

        public string Veildatum { get; set; } = "";
        // Datum van de veiling (geformatteerd als string)

        public string StartTijd { get; set; } = "";
        // Starttijd van de veiling

        public string EindTijd { get; set; } = "";
        // Eindtijd van de veiling

        public int AantalProducten { get; set; }
        // Totaal aantal producten in deze veiling

        public List<VeilingTransactieDto> Transacties { get; set; } = new();
        // Alle transacties die tijdens de veiling zijn uitgevoerd
    }

    // DTO voor één verkoop/transactie binnen een veiling
    public class VeilingTransactieDto
    {
        public string KoperNaam { get; set; } = "";
        // Naam van de koper

        public string Soort { get; set; } = "";
        // Soort product dat verkocht is

        public int Aantal { get; set; }
        // Aantal verkochte eenheden

        public decimal Prijs { get; set; }
        // Verkoopprijs

        public DateTime Tijdstip { get; set; }
        // Moment waarop de transactie is afgerond
    }
}
