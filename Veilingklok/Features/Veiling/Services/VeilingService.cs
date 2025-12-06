using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Shared;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Veiling.Services;

public sealed class VeilingService : IVeilingService
{
    private readonly MyContext _db;

    public VeilingService(MyContext db)//Dependency Injection
    {
        _db = db;
    }

    public async Task<Result<VeilingProduct>> GetCurrentProductAsync(int veilingId)//het actuele product
    {
        //Query
        //veiling + actieve product + productdetails + aanvoerder binnenhalen
        var veiling = await _db.Veilingen
            .Include(v => v.CurrentVeilingProduct)
                .ThenInclude(p => p.Product)
            .Include(v => v.CurrentVeilingProduct)
                .ThenInclude(p => p.Aanvoerder)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        
        
        
        
        
        
        //foutmelding want id bestaat niet
        if (veiling == null)
            return Result<VeilingProduct>.Fail("Veiling niet gevonden.");

        //veiling nog niet gestart en geen CurrentVeilingProduct.
        if (veiling.CurrentVeilingProduct == null)
            return Result<VeilingProduct>.Fail("Geen actief product.");

        //Geeft het actieve product terug
        return Result<VeilingProduct>.Ok(veiling.CurrentVeilingProduct);
    }

    
    
    
    
    
    //alle producten die nog moeten komen
    public async Task<Result<List<VeilingProduct>>> GetQueueAsync(int veilingId)
    {
        var producten = await _db.VeilingProducten
            .Include(p => p.Product)
            .Include(p => p.Aanvoerder)
            .Where(p => p.VeilingId == veilingId && p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .ToListAsync();

        return Result<List<VeilingProduct>>.Ok(producten);
    }

    
    
    
    
    
    
    
    
    
    
    ////een bod op het actieve produc platsen
    public async Task<Result<Bid>> PlaceBidAsync(int veilingId, int koperId, decimal amount)
    {
        
        //Haal veiling + actief product op
        var veiling = await _db.Veilingen
            .Include(v => v.CurrentVeilingProduct)
            .FirstOrDefaultAsync(v => v.Id == veilingId);

        
        
        
        
        //bestaat de veiling?
        if (veiling == null)
            return Result<Bid>.Fail("Veiling niet gevonden.");

        
        
        
        
        //is er een actief product
        var current = veiling.CurrentVeilingProduct;
        if (current == null || current.Status != VeilingProductStatus.Active)
            return Result<Bid>.Fail("Bieden is niet mogelijk.");

        
        
        
        
        // een nieuwe bod aanmaken
        var bid = new Bid
        {
            VeilingId = veilingId,
            VeilingProductId = current.Id,
            KoperId = koperId,
            PlacedByGebruikerId = koperId,
            Amount = amount,
            Source = BidSource.Buyer
        };

        
        
        
        
        
        current.Bids.Add(bid);//Bod toevoegen aan het product
        current.HuidigePrijs = amount;//Prijs updaten

        await _db.SaveChangesAsync();//Opslaan in db

        return Result<Bid>.Ok(bid);//terugsturen naar frontend
    }
}