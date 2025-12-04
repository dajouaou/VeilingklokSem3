namespace Veilingklok.Core.Entities;

public class Aanvoerder
{
    public int Id { get; set; }
    public string Naam { get; set; } = "";
    public string Email { get; set; } = "";

    public ICollection<Aanmelding> Aanmeldingen { get; set; } = new List<Aanmelding>();
}
