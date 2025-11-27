using Microsoft.EntityFrameworkCore;
using Veilingklok.ModelKlassen; // 

namespace Veilingklok.Infrastructure.Database
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        // -------------------------------------------
        // DbSets  Tabellen in database
        // -------------------------------------------
        public DbSet<Gebruiker> Gebruikers => Set<Gebruiker>();
        public DbSet<Koper> Kopers => Set<Koper>();
        public DbSet<Aanvoerder> Aanvoerders => Set<Aanvoerder>();
        public DbSet<Product> Producten => Set<Product>();
        public DbSet<Veiling> Veilingen => Set<Veiling>();
        public DbSet<VeilingProduct> VeilingProducten => Set<VeilingProduct>();
        public DbSet<Bod> Biedingen => Set<Bod>();

        // -------------------------------------------
        // Fluent API configuratie
        // -------------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Laad alle configuratie klassen in de map "Configurations"
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyContext).Assembly);
        }
    }
}