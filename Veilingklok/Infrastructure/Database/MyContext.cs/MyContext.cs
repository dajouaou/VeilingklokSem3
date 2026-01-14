using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database;

public class MyContext : DbContext
{
    // DbContext die alle tabellen van het veilingplatform bij elkaar brengt
    public MyContext(DbContextOptions<MyContext> options) : base(options) { }

    // Gebruikers en rollen
    public DbSet<Gebruiker> Gebruikers => Set<Gebruiker>();
    public DbSet<Koper> Kopers => Set<Koper>();
    public DbSet<Aanvoerder> Aanvoerders => Set<Aanvoerder>();
    public DbSet<Veilingmeester> Veilingmeesters => Set<Veilingmeester>();

    // Aanmeldingen en veilingen
    public DbSet<Aanmelding> Aanmeldingen => Set<Aanmelding>();
    public DbSet<Veiling> Veilingen => Set<Veiling>();
    public DbSet<VeilingProduct> VeilingProducten => Set<VeilingProduct>();

    // Biedingen en transacties
    public DbSet<Bod> Biedingen => Set<Bod>();
    public DbSet<Transactie> Transacties => Set<Transactie>();

    // Extra tabellen
    public DbSet<Veildag> Veildagen { get; set; }
    public DbSet<Product> Producten => Set<Product>();

    // Laadt automatisch alle entity-configuraties uit dit project
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
