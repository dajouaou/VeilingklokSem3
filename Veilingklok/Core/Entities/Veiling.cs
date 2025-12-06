using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Veiling
{
    public int Id { get; set; }//pk

    public int VMId { get; set; }//fk
    public VM? VM { get; set; }//vm zelf

    public VeilingStatus Status { get; set; } = VeilingStatus.Draft;//status

    //wanneer de v moet beginnen
    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;//wanneer is v aangemaakt

    public string? Locatie { get; set; }//locatie van v

    public int? CurrentVeilingProductId { get; set; }//fk van product dat nu geveild wordt
    public VeilingProduct? CurrentVeilingProduct { get; set; }//EF navigatie naar het actuele productobject

    public List<VeilingProduct> VeilingProducten { get; set; } = new();//de queue
    public List<Bid> Bids { get; set; } = new();//Alle biedingen
    public List<AuditEntry> AuditEntries { get; set; } = new();//Logboek

    public byte[]? RowVersion { get; set; }//concurrency token.
}