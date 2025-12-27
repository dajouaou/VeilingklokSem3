using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options) : base(options) { }

    public DbSet<Gebruiker> Gebruikers => Set<Gebruiker>();
    public DbSet<Koper> Kopers => Set<Koper>();
    public DbSet<Aanvoerder> Aanvoerders => Set<Aanvoerder>();
    public DbSet<VM> Veilingmeesters => Set<VM>();
    public DbSet<Aanmelding> Aanmeldingen => Set<Aanmelding>();
    public DbSet<Veiling> Veilingen => Set<Veiling>();
    public DbSet<VeilingProduct> VeilingProducten => Set<VeilingProduct>();
    public DbSet<Bod> Biedingen => Set<Bod>();
    public DbSet<Veildag> Veildagen { get; set; }
    public DbSet<Product> Producten => Set<Product>();




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyContext).Assembly);
    }
}