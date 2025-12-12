using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

public class Aanmelding
{
    public int Id { get; set; }

    public int AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    public string Soort { get; set; } = string.Empty;
    public string? Potmaat { get; set; }
    public string? Steellengte { get; set; }
    public int Hoeveelheid { get; set; }
    public decimal MinimumPrijs { get; set; }

    public KlokLocatie KlokLocatie { get; set; }
    public DateTime LeverDatum { get; set; }

    public string? FotoUrl { get; set; }
    public int? VeilingProductId { get; set; }
    public VeilingProduct? VeilingProduct { get; set; }
    public string? Beschrijving { get; set; }


}
