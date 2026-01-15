namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO met de benodigde gegevens om een veiling te starten
    public class StartVeilingDto
    {
        public DateTime Veildatum { get; set; }
        // Datum waarop de veiling plaatsvindt

        public DateTime LeverDatum { get; set; }
        // Datum waarop de producten geleverd worden

        public TimeSpan? StartTijd { get; set; }
        // Optionele starttijd van de veiling (null = direct starten of standaardtijd)
    }
}
