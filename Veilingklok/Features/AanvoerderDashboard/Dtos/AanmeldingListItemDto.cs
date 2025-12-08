public class AanmeldingListItemDto
{
    public int Id { get; set; }
    public string Soort { get; set; } = string.Empty;

    public string? Potmaat { get; set; }
    public string? Steellengte { get; set; }

    public int Hoeveelheid { get; set; }
    public decimal MinimumPrijs { get; set; }

    public string KlokLocatie { get; set; } = string.Empty;
    public DateTime Veildatum { get; set; }

    public string? FotoUrl { get; set; }

    public bool IsVerkocht { get; set; }
    public string? KoperNaam { get; set; }
    public decimal? VerkoopPrijs { get; set; }
    public decimal? TotaleOpbrengst { get; set; }
}
