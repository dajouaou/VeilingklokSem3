namespace Veilingklok.Features.Kopers.Dtos
{
    public class KoperDTO
    {
        public int Id { get; set; }
        public string GebruikerUsername { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public int BiedingenCount { get; set; }
    }
}
