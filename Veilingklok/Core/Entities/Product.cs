namespace Veilingklok.Core.Entities;

/// <summary>
/// Basisartikel (bloem/plant) dat door een Aanvoerder wordt aangeboden.
/// Wordt gebruikt als “producttype”; het daadwerkelijke lot zit in VeilingProduct.
/// </summary>
public class Product
{
    // ------------------------------------------------------------
    // Primary Key
    // ------------------------------------------------------------
    public int Id { get; set; }                         // PK

    // ------------------------------------------------------------
    // Herkomst / Aanvoerder (kweker)
    // ------------------------------------------------------------

    /// <summary>
    /// FK → Aanvoerder. 
    /// Dit geeft aan wie het product kweekt (originele eigenaar).
    /// </summary>
    public int AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    // ------------------------------------------------------------
    // Basis artikelinfo (UI + case-eisen)
    // ------------------------------------------------------------

    /// <summary>
    /// Commerciële productnaam (bijv. 'Roos Avalanche', 'Ficus Elastica').
    /// </summary>
    public string Naam { get; set; } = string.Empty;

    /// <summary>
    /// Algemene categorie (bijv. 'Rozen', 'Kamerplanten', 'Tulpen').
    /// </summary>
    public string? Categorie { get; set; }

    /// <summary>
    /// Beschrijving van het product (korte marketing- of teeltinformatie).
    /// </summary>
    public string? Beschrijving { get; set; }

    /// <summary>
    /// Foto van het product (getoond op Veiling UI en Actuele Product).
    /// </summary>
    public string? FotoUrl { get; set; }

    // ------------------------------------------------------------
    // Artikelkenmerken (aanbevolen door de case)
    // Minimale set om “artikelkenmerken” zichtbaar te maken.
    // ------------------------------------------------------------

    /// <summary>
    /// Kleuren van het product (bijv. 'Rood', 'Wit', 'Roze').
    /// </summary>
    public string? Kleur { get; set; }

    /// <summary>
    /// Hoogte van de steel/plant, bijv. '40cm', '60cm'.
    /// </summary>
    public string? Hoogte { get; set; }

    /// <summary>
    /// Aantal stelen of planten per bos / tray.
    /// </summary>
    public int? AantalPerBos { get; set; }

    // ------------------------------------------------------------
    // Relaties → loten binnen veilingen
    // ------------------------------------------------------------

    /// <summary>
    /// Alle concrete veilingloten die gebaseerd zijn op dit product.
    /// </summary>
    public List<VeilingProduct> VeilingProducten { get; set; } = new();
}
