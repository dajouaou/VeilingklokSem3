namespace Veilingklok.Features.Veiling.Dtos
{
    // DTO met het totale overzicht van een veiling
    public class VeilingOverzichtDto
    {
        public int Id { get; set; }
        // ID van de veiling

        public bool IsGestart { get; set; }
        // Geeft aan of de veiling gestart is

        public bool IsPauze { get; set; }
        // Geeft aan of de veiling momenteel gepauzeerd is

        public bool IsAfgesloten { get; set; }
        // Geeft aan of de veiling is afgerond

        public HuidigProductDto? HuidigProduct { get; set; }
        // Het product dat op dit moment geveild wordt (null als er geen actief product is)

        public List<WachtrijItemDto> Wachtrij { get; set; } = new();
        // Lijst met producten die nog in de wachtrij staan
    }
}
