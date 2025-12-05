using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Infrastructure.Database;
using Veilingklok.Core.Entities;


namespace Veilingklok.Infrastructure.Database.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(MyContext db)
    {
        if (await db.Gebruikers.AnyAsync()) return;

        var now = DateTime.UtcNow;

        string Img(string file) => $"/img/products/{file}";
        string Hash(string pw) => BCrypt.Net.BCrypt.HashPassword(pw);

        // 1) Gebruikers (10)

        var sofiaMeester = new Gebruiker
        {
            Username = "sofia.veilingmeester",
            Email = "sofia.veilingmeester@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Veilingmeester,
            CreatedAtUtc = now
        };

        var sofiaAdmin = new Gebruiker
        {
            Username = "sofia.admin",
            Email = "sofia.admin@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Admin,
            CreatedAtUtc = now
        };

        var saraUser = new Gebruiker
        {
            Username = "sara.aanvoerder",
            Email = "sara@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Aanvoerder,
            CreatedAtUtc = now
        };

        var soniaUser = new Gebruiker
        {
            Username = "sonia.aanvoerder",
            Email = "sonia@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Aanvoerder,
            CreatedAtUtc = now
        };

        var siaUser = new Gebruiker
        {
            Username = "sia.aanvoerder",
            Email = "sia@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Aanvoerder,
            CreatedAtUtc = now
        };

        var sashaUser = new Gebruiker
        {
            Username = "sasha.koper",
            Email = "sasha@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var saynaUser = new Gebruiker
        {
            Username = "sayna.koper",
            Email = "sayna@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var chrisUser = new Gebruiker
        {
            Username = "chris.koper",
            Email = "chris@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var tristanUser = new Gebruiker
        {
            Username = "tristan.koper",
            Email = "tristan@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        var miriamUser = new Gebruiker
        {
            Username = "miriam.koper",
            Email = "miriam@veilingklok.local",
            PasswordHash = Hash("Pass123!"),
            Rol = UserRole.Koper,
            CreatedAtUtc = now
        };

        db.Gebruikers.AddRange(
            sofiaMeester, sofiaAdmin,
            saraUser, soniaUser, siaUser,
            sashaUser, saynaUser, chrisUser, tristanUser, miriamUser
        );
        await db.SaveChangesAsync();

        // -----------------------------
        // 2) Profielen (3 + 5)
        // -----------------------------
        var saraAanvoerder = new Aanvoerder { GebruikerId = saraUser.Id, Naam = "Sara Pasargad" };
        var soniaAanvoerder = new Aanvoerder { GebruikerId = soniaUser.Id, Naam = "Sonia Pasarga" };
        var siaAanvoerder = new Aanvoerder { GebruikerId = siaUser.Id, Naam = "Sia Zagros" };

        var sashaKoper = new Koper { GebruikerId = sashaUser.Id, Naam = "Sasha Niki" };
        var saynaKoper = new Koper { GebruikerId = saynaUser.Id, Naam = "Sayna Shayan" };
        var chrisKoper = new Koper { GebruikerId = chrisUser.Id, Naam = "Chris Shayan" };
        var tristanKoper = new Koper { GebruikerId = tristanUser.Id, Naam = "Tristan Perspolisi" };
        var miriamKoper = new Koper { GebruikerId = miriamUser.Id, Naam = "Miriam van de Hoeven" };

        db.Aanvoerders.AddRange(saraAanvoerder, soniaAanvoerder, siaAanvoerder);
        db.Kopers.AddRange(sashaKoper, saynaKoper, chrisKoper, tristanKoper, miriamKoper);
        await db.SaveChangesAsync();

        // -----------------------------
        // 3) Producten (18)
        // -----------------------------
        var producten = new List<Product>
        {
            // Sara
            new() { AanvoerderId = saraAanvoerder.Id, Naam = "Alstroemeria mix", Categorie = "Snijbloemen", Beschrijving = "Kleurrijke mix, vers aangevoerd.", FotoUrl = Img("alstroemeria-mix.jpg") },
            new() { AanvoerderId = saraAanvoerder.Id, Naam = "Anthurium rood", Categorie = "Snijbloemen", Beschrijving = "Dieprood, stevige steel.", FotoUrl = Img("anthurium-rood.jpg") },
            new() { AanvoerderId = saraAanvoerder.Id, Naam = "Calla lily oranje", Categorie = "Snijbloemen", Beschrijving = "Oranje calla, premium.", FotoUrl = Img("calla-lily-oranje.jpg") },
            new() { AanvoerderId = saraAanvoerder.Id, Naam = "Gypsophila wit", Categorie = "Snijbloemen", Beschrijving = "Gipskruid wit, mooie vulling.", FotoUrl = Img("gypsophila-paniculata-wit.jpg") },
            new() { AanvoerderId = saraAanvoerder.Id, Naam = "Tulpen dubbelbloemig mix", Categorie = "Snijbloemen", Beschrijving = "Seizoensmix tulpen.", FotoUrl = Img("tulpen-dubbelbloemig-mix.jpg") },
            new() { AanvoerderId = saraAanvoerder.Id, Naam = "Boeket ranunculus mix (premium)", Categorie = "Boeketten", Beschrijving = "Premium boeket.", FotoUrl = Img("boeket-ranunculus-mix-premium.jpg") },

            // Sonia
            new() { AanvoerderId = soniaAanvoerder.Id, Naam = "Ficus lyrata (Ø21)", Categorie = "Planten", Beschrijving = "Kamerplant Ø21.", FotoUrl = Img("ficus-lyrata.jpg") },
            new() { AanvoerderId = soniaAanvoerder.Id, Naam = "Sansevieria zeylanica", Categorie = "Planten", Beschrijving = "Sterk en makkelijk.", FotoUrl = Img("sansevieria-zeylanica.jpg") },
            new() { AanvoerderId = soniaAanvoerder.Id, Naam = "Orchidee phalaenopsis wit", Categorie = "Planten", Beschrijving = "Wit (2-tak).", FotoUrl = Img("orchidee-phalaenopsis-wit.jpg") },
            new() { AanvoerderId = soniaAanvoerder.Id, Naam = "Ficus lyrata (batch 2)", Categorie = "Planten", Beschrijving = "Tweede batch.", FotoUrl = Img("ficus-lyrata.jpg") },
            new() { AanvoerderId = soniaAanvoerder.Id, Naam = "Sansevieria zeylanica (klein)", Categorie = "Planten", Beschrijving = "Compact formaat.", FotoUrl = Img("sansevieria-zeylanica.jpg") },
            new() { AanvoerderId = soniaAanvoerder.Id, Naam = "Orchidee wit (cadeau)", Categorie = "Planten", Beschrijving = "Cadeauproof.", FotoUrl = Img("orchidee-phalaenopsis-wit.jpg") },

            // Sia
            new() { AanvoerderId = siaAanvoerder.Id, Naam = "Boeket gerbera mix", Categorie = "Boeketten", Beschrijving = "Vrolijke mix.", FotoUrl = Img("boeket-gerbera-mix.jpg") },
            new() { AanvoerderId = siaAanvoerder.Id, Naam = "Seizoensboeket herfstmix", Categorie = "Boeketten", Beschrijving = "Herfst kleuren.", FotoUrl = Img("seizoensboeket-herfst-mix.jpg") },
            new() { AanvoerderId = siaAanvoerder.Id, Naam = "White Rose & Lisianthus Bouquet", Categorie = "Boeketten", Beschrijving = "Witte mix.", FotoUrl = Img("White-Rose-Lisianthus-Bouquet.jpg") },
            new() { AanvoerderId = siaAanvoerder.Id, Naam = "Alstroemeria mix (extra)", Categorie = "Snijbloemen", Beschrijving = "Extra batch.", FotoUrl = Img("alstroemeria-mix.jpg") },
            new() { AanvoerderId = siaAanvoerder.Id, Naam = "Anthurium rood (selectie)", Categorie = "Snijbloemen", Beschrijving = "Selectiepartij.", FotoUrl = Img("anthurium-rood.jpg") },
            new() { AanvoerderId = siaAanvoerder.Id, Naam = "Boeket gerbera mix (premium)", Categorie = "Boeketten", Beschrijving = "Premium mix.", FotoUrl = Img("boeket-gerbera-mix.jpg") },
        };

        db.Producten.AddRange(producten);
        await db.SaveChangesAsync();

        // -----------------------------
        // helper lots (✅ bevat AanvoerderId)
        // -----------------------------
        VeilingProduct Lot(
            int veilingId,
            Product p,
            int volgorde,
            VeilingProductStatus status,
            decimal start,
            decimal current,
            int? soldToKoperId = null,
            DateTime? activated = null,
            DateTime? closed = null
        ) => new()
        {
            VeilingId = veilingId,
            ProductId = p.Id,
            AanvoerderId = p.AanvoerderId, // ✅ FIX
            Volgorde = volgorde,
            Hoeveelheid = volgorde % 3 + 1,
            StartPrijs = start,
            HuidigePrijs = current,
            Status = status,
            SoldToKoperId = soldToKoperId,
            ActivatedAtUtc = activated,
            ClosedAtUtc = closed,
        };

        // -----------------------------
        // 4) Veilingen (2) + lots (20)
        // -----------------------------
        var veilingA = new Veiling
        {
            Status = VeilingStatus.Running,
            StartTijdUtc = now.AddMinutes(-30),
            EindTijdUtc = now.AddHours(2),
            CreatedAtUtc = now
        };

        var veilingB = new Veiling
        {
            Status = VeilingStatus.Scheduled,
            StartTijdUtc = now.AddHours(3),
            EindTijdUtc = now.AddHours(5),
            CreatedAtUtc = now
        };

        db.Veilingen.AddRange(veilingA, veilingB);
        await db.SaveChangesAsync();

        var aProducts = producten.Take(10).ToList();
        var bProducts = producten.Skip(8).Take(10).ToList();

        var aLots = new List<VeilingProduct>
        {
            Lot(veilingA.Id, aProducts[0], 1,  VeilingProductStatus.Queued,   0.80m, 0.80m),
            Lot(veilingA.Id, aProducts[1], 2,  VeilingProductStatus.Queued,   0.90m, 0.90m),
            Lot(veilingA.Id, aProducts[2], 3,  VeilingProductStatus.Queued,   1.00m, 1.00m),
            Lot(veilingA.Id, aProducts[3], 4,  VeilingProductStatus.Queued,   1.10m, 1.10m),
            Lot(veilingA.Id, aProducts[4], 5,  VeilingProductStatus.Queued,   1.20m, 1.20m),
            Lot(veilingA.Id, aProducts[5], 6,  VeilingProductStatus.Queued,   1.30m, 1.30m),

            Lot(veilingA.Id, aProducts[6], 7,  VeilingProductStatus.Active,   1.50m, 2.10m, activated: now.AddMinutes(-10)),

            Lot(veilingA.Id, aProducts[7], 8,  VeilingProductStatus.Sold,     1.40m, 1.95m, soldToKoperId: saynaKoper.Id, closed: now.AddMinutes(-6)),
            Lot(veilingA.Id, aProducts[8], 9,  VeilingProductStatus.Sold,     1.00m, 1.60m, soldToKoperId: miriamKoper.Id, closed: now.AddMinutes(-4)),

            Lot(veilingA.Id, aProducts[9], 10, VeilingProductStatus.Skipped,  0.70m, 0.70m, closed: now.AddMinutes(-2)),
        };

        var bLots = bProducts.Select((p, idx) =>
            Lot(
                veilingB.Id,
                p,
                idx + 1,
                VeilingProductStatus.Queued,
                0.80m + (idx * 0.10m),
                0.80m + (idx * 0.10m)
            )
        ).ToList();

        db.VeilingProducten.AddRange(aLots);
        db.VeilingProducten.AddRange(bLots);
        await db.SaveChangesAsync();

        var activeLotA = aLots.Single(x => x.Status == VeilingProductStatus.Active);
        veilingA.CurrentVeilingProductId = activeLotA.Id;
        await db.SaveChangesAsync();

        // -----------------------------
        // 5) Bids (±12)
        // -----------------------------
        var bidTimes = Enumerable.Range(0, 9).Select(i => now.AddMinutes(-9 + i)).ToList();
        var amounts = new[] { 1.50m, 1.60m, 1.70m, 1.80m, 1.90m, 2.00m, 2.05m, 2.10m, 2.12m };

        var buyerPairs = new (Gebruiker user, Koper koper)[]
        {
            (sashaUser, sashaKoper),
            (saynaUser, saynaKoper),
            (chrisUser, chrisKoper),
            (tristanUser, tristanKoper),
            (miriamUser, miriamKoper),
            (sashaUser, sashaKoper),
            (saynaUser, saynaKoper),
            (chrisUser, chrisKoper),
        };

        var bids = new List<Bid>();

        for (var i = 0; i < buyerPairs.Length; i++)
        {
            var (u, k) = buyerPairs[i];
            bids.Add(new Bid
            {
                VeilingId = veilingA.Id,
                VeilingProductId = activeLotA.Id,
                PlacedByGebruikerId = u.Id,
                KoperId = k.Id,
                Amount = amounts[i],
                Source = BidSource.Buyer,
                PlacedAtUtc = bidTimes[i]
            });
        }

        bids.Add(new Bid
        {
            VeilingId = veilingA.Id,
            VeilingProductId = activeLotA.Id,
            PlacedByGebruikerId = sofiaMeester.Id,
            KoperId = null,
            Amount = amounts[8],
            Source = BidSource.Auctioneer,
            PlacedAtUtc = bidTimes[8]
        });

        var soldLot = aLots.First(x => x.Status == VeilingProductStatus.Sold);
        bids.Add(new Bid
        {
            VeilingId = veilingA.Id,
            VeilingProductId = soldLot.Id,
            PlacedByGebruikerId = saynaUser.Id,
            KoperId = saynaKoper.Id,
            Amount = 1.70m,
            Source = BidSource.Buyer,
            PlacedAtUtc = now.AddMinutes(-20)
        });

        bids.Add(new Bid
        {
            VeilingId = veilingA.Id,
            VeilingProductId = soldLot.Id,
            PlacedByGebruikerId = saynaUser.Id,
            KoperId = saynaKoper.Id,
            Amount = 1.95m,
            Source = BidSource.Buyer,
            PlacedAtUtc = now.AddMinutes(-19)
        });

        db.Biedingen.AddRange(bids);

        activeLotA.HuidigePrijs = bids
            .Where(b => b.VeilingProductId == activeLotA.Id)
            .Max(b => b.Amount);

        await db.SaveChangesAsync();

        // -----------------------------
        // 6) Audit (16)
        // -----------------------------
        var audits = new List<AuditEntry>();

        void AddAudit(int veilingId, int actorUserId, string action, string? details, DateTime at)
            => audits.Add(new AuditEntry
            {
                VeilingId = veilingId,
                ActorGebruikerId = actorUserId,
                Action = action,
                Details = details,
                CreatedAtUtc = at
            });

        AddAudit(veilingA.Id, sofiaMeester.Id, "AUCTION_START", "Veiling gestart (seed).", now.AddMinutes(-30));
        AddAudit(veilingA.Id, sofiaMeester.Id, "LOT_ACTIVATED", $"Lot {activeLotA.Id} actief.", now.AddMinutes(-10));
        AddAudit(veilingA.Id, sashaUser.Id, "BID_PLACED", $"Bid {amounts[0]:0.00} op lot {activeLotA.Id}.", now.AddMinutes(-9));
        AddAudit(veilingA.Id, saynaUser.Id, "BID_PLACED", $"Bid {amounts[1]:0.00} op lot {activeLotA.Id}.", now.AddMinutes(-8));
        AddAudit(veilingA.Id, chrisUser.Id, "BID_PLACED", $"Bid {amounts[2]:0.00} op lot {activeLotA.Id}.", now.AddMinutes(-7));
        AddAudit(veilingA.Id, sofiaMeester.Id, "BID_PLACED", $"Auctioneer bid {amounts[8]:0.00} op lot {activeLotA.Id}.", now.AddMinutes(-1));
        AddAudit(veilingA.Id, sofiaMeester.Id, "LOT_SOLD", $"Sold lot {soldLot.Id} (seed).", now.AddMinutes(-6));
        AddAudit(veilingA.Id, sofiaMeester.Id, "LOT_SKIPPED", "Skipped lot (seed).", now.AddMinutes(-2));

        AddAudit(veilingB.Id, sofiaMeester.Id, "AUCTION_CREATED", "Veiling gepland (seed).", now.AddMinutes(-5));
        AddAudit(veilingB.Id, sofiaAdmin.Id, "ADMIN_NOTE", "Admin check (seed).", now.AddMinutes(-4));
        AddAudit(veilingB.Id, sofiaMeester.Id, "QUEUE_READY", "Queue klaar voor veiling B.", now.AddMinutes(-3));
        AddAudit(veilingB.Id, sofiaMeester.Id, "STATUS_SET", "Status = Scheduled.", now.AddMinutes(-2));
        AddAudit(veilingB.Id, sofiaMeester.Id, "INFO", "Testdata voor dashboard UI.", now.AddMinutes(-1));
        AddAudit(veilingB.Id, sofiaMeester.Id, "INFO", "Products per aanvoerder aanwezig.", now);
        AddAudit(veilingB.Id, sofiaMeester.Id, "INFO", "Bids aanwezig op veiling A.", now);
        AddAudit(veilingB.Id, sofiaMeester.Id, "INFO", "Current lot bestaat op veiling A.", now);

        db.AuditEntries.AddRange(audits);
        await db.SaveChangesAsync();
    }
}
