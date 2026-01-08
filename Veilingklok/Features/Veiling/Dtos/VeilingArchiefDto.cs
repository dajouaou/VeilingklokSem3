namespace Veilingklok.Features.Veiling.Dtos
{
    public class VeilingArchiefDto
    {
        public int Id { get; set; }
        public string Veildatum { get; set; } = "";  
        public string StartTijd { get; set; } = "";
        public string EindTijd { get; set; } = "";
        public int AantalProducten { get; set; }

        public List<VeilingTransactieDto> Transacties { get; set; } = new();
    }

    public class VeilingTransactieDto
    {
        public string KoperNaam { get; set; } = "";
        public string Soort { get; set; } = "";
        public int Aantal { get; set; }
        public decimal Prijs { get; set; }
        public DateTime Tijdstip { get; set; }
    }
}
