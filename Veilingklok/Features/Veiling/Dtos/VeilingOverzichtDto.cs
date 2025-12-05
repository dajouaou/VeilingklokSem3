namespace Veilingklok.Features.Veiling.Dtos
{
    public class VeilingOverzichtDto
    {
        public int Id { get; set; }
        public bool IsGestart { get; set; }
        public bool IsPauze { get; set; }
        public bool IsAfgesloten { get; set; }

        public HuidigProductDto? HuidigProduct { get; set; }
        public List<WachtrijItemDto> Wachtrij { get; set; } = new();
    }
}
