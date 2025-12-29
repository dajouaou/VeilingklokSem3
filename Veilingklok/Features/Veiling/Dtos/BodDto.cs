namespace Veilingklok.Features.Veiling.Dtos
{
    public class BodDto
    {
        public int Id { get; set; }
        public decimal Prijs { get; set; }
        public string? KoperNaam { get; set; }
        public DateTime Tijdstip { get; set; }
    }
}

