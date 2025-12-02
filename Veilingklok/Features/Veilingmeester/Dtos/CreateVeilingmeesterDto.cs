namespace Veilingklok.Features.Veilingmeester.Dtos;

public sealed class CreateVeilingmeesterDto
{
    public int GebruikerId { get; set; }
    public string Naam { get; set; } = string.Empty;
}