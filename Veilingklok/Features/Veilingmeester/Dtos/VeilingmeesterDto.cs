namespace Veilingklok.Features.Veilingmeester.Dtos;

public sealed class VeilingmeesterDto
{
    public int Id { get; set; }
    public int GebruikerId { get; set; }
    public string Naam { get; set; } = string.Empty;

    public List<int> VeilingIds { get; set; } = new();
}