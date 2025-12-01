using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

/// <summary>
/// Een concreet lot in een specifieke Veiling.
/// Combineert een Product + hoeveelheid + prijs + status.
/// Dit is het hart van de digitale veilingklok.
/// </summary>
public class VeilingProduct
{
    // =====================================================================
    // Primary Key
    // =====================================================================
    public int Id { get; set; }

    // =====================================================================
    // Relaties
    // =====================================================================

    /// <summary>
    /// De veiling waarin dit lot wordt verkocht.
    /// </summary>
    public int VeilingId { get; set; }
    public Veiling? Veiling { get; set; }

    /// <summary>
    /// Basisproduct (bloem/plant).
    /// </summary>
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    /// <summary>
    /// Snapshot: aanvoerder die dit product op DIT moment aanbiedt.
    /// Belangrijk omdat producten soms door meerdere aanvoerders worden aangeboden.
    /// (Vermijdt dat historische data verandert als het Product wordt aangepast.)
    /// </summary>
    public int? AanvoerderId { get; set; }
    public Aanvoerder? Aanvoerder { get; set; }

    // =====================================================================
    // Veiling-specificatie
    // =====================================================================

    /// <summary>
    /// De positie in de veilingvolgorde (1 = eerst).
    /// </summary>
    public int Volgorde { get; set; }

    /// <summary>
    /// Hoeveelheid in stuks/emmer/tray. Wordt getoond op de klok.
    /// </summary>
    public int Hoeveelheid { get; set; } = 1;

    /// <summary>
    /// Startprijs ingesteld door de veilingmeester voordat het lot actief wordt.
    /// </summary>
    public decimal StartPrijs { get; set; }

    /// <summary>
    /// De actuele prijs tijdens het bieden.
    /// Wordt bijgewerkt via service + SignalR events.
    /// </summary>
    public decimal HuidigePrijs { get; set; }

    /// <summary>
    /// Statusverloop:
    /// Queued → Active → Sold → Closed.
    /// </summary>
    public VeilingProductStatus Status { get; set; } = VeilingProductStatus.Queued;

    /// <summary>
    /// Tijdstip waarop dit lot "Active" werd.
    /// </summary>
    public DateTime? ActivatedAtUtc { get; set; }

    /// <summary>
    /// Tijdstip waarop het lot gesloten/verkocht werd.
    /// </summary>
    public DateTime? ClosedAtUtc { get; set; }

    // =====================================================================
    // Verkoopresultaten
    // =====================================================================

    /// <summary>
    /// De koper die dit lot uiteindelijk heeft gewonnen.
    /// </summary>
    public int? SoldToKoperId { get; set; }
    public Koper? SoldToKoper { get; set; }

    // =====================================================================
    // Biedingen
    // =====================================================================

    /// <summary>
    /// Alle biedingen die op dit veilinglot zijn gedaan.
    /// </summary>
    public List<Bid> Bids { get; set; } = new();
}
