namespace Veilingklok.Features.AanvoerderDashboard.Dtos;

// dto voor dashboard: toont info van aanvoerder + productstatistieken
public sealed class AanvoerderDto
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }

    // dashboard extra’s
    public int ProductCount { get; set; } = 0;         // totaal producten
    public List<string> Categorieen { get; set; } = new(); // unieke categorieën van zijn producten
}