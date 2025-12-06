using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class VeilingProduct
{
    public int Id { get; set; }//pk

    public int VeilingId { get; set; }//fk
    public Veiling? Veiling { get; set; }//veiling zelf

    public int ProductId { get; set; }// fk naar product
    public Product? Product { get; set; }//product zelf

    public int? AanvoerderId { get; set; }//fk naar aanvoerder
    public Aanvoerder? Aanvoerder { get; set; }//aanvoerder

    public int Volgorde { get; set; }//plek in de queue
    public int Hoeveelheid { get; set; } = 1;//Hoeveel stuks

    public decimal StartPrijs { get; set; }//De prijs waarmee de klok begint voordat het aftellen start
    public decimal HuidigePrijs { get; set; }//prijs die nu geld

    public VeilingProductStatus Status { get; set; } = VeilingProductStatus.Queued;//status van  veilingproduct

    public DateTime? ActivatedAtUtc { get; set; }//wanneer  actief wordt
    public DateTime? ClosedAtUtc { get; set; }//gestopt

    public int? SoldToKoperId { get; set; }//fk naar koper
    public Koper? SoldToKoper { get; set; }//navigatie naar koper

    public List<Bid> Bids { get; set; } = new();//geplaatste biedingen
}
 