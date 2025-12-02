namespace Veilingklok.Features.Producten.Dtos;

public sealed class SearchProductDto
{
    public string? Naam { get; set; }
    public string? Categorie { get; set; }
    public string? Kleur { get; set; }
    public int? AanvoerderId { get; set; }

    // Extra filters
    public string? HoogteMin { get; set; }
    public string? HoogteMax { get; set; }

    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}