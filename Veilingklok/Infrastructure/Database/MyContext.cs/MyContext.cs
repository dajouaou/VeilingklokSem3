using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options) : base(options) { }

   
    public DbSet<Gebruiker> Gebruikers => Set<Gebruiker>();
    public DbSet<Koper> Kopers => Set<Koper>();
    public DbSet<Aanvoerder> Aanvoerders => Set<Aanvoerder>();
    public DbSet<VM> VMs => Set<VM>();

    
    public DbSet<Product> Producten => Set<Product>();
    public DbSet<Veiling> Veilingen => Set<Veiling>();
    public DbSet<VeilingProduct> VeilingProducten => Set<VeilingProduct>();

   
    public DbSet<Bid> Biedingen => Set<Bid>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Gebruiker>()
            .HasOne(g => g.Koper)
            .WithOne(k => k.Gebruiker)
            .HasForeignKey<Koper>(k => k.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Gebruiker>()
            .HasOne(g => g.Aanvoerder)
            .WithOne(a => a.Gebruiker)
            .HasForeignKey<Aanvoerder>(a => a.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Gebruiker>()
            .HasOne(g => g.VM)
            .WithOne(vm => vm.Gebruiker)
            .HasForeignKey<VM>(vm => vm.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);


   
        
        modelBuilder.Entity<VM>()
            .HasMany(vm => vm.Veilingen)
            .WithOne(v => v.VM)
            .HasForeignKey(v => v.VMId)
            .OnDelete(DeleteBehavior.Restrict);


        
        modelBuilder.Entity<Aanvoerder>()
            .HasMany(a => a.Producten)
            .WithOne(p => p.Aanvoerder)
            .HasForeignKey(p => p.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);


       
        modelBuilder.Entity<Veiling>()
            .HasMany(v => v.VeilingProducten)
            .WithOne(vp => vp.Veiling)
            .HasForeignKey(vp => vp.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);


    
        modelBuilder.Entity<VeilingProduct>()
            .HasOne(vp => vp.Product)
            .WithMany()
            .HasForeignKey(vp => vp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);


   
        modelBuilder.Entity<VeilingProduct>()
            .HasOne(vp => vp.Aanvoerder)
            .WithMany(a => a.VeilingProducten)
            .HasForeignKey(vp => vp.AanvoerderId)
            .OnDelete(DeleteBehavior.SetNull);


  
        modelBuilder.Entity<Bid>()
            .HasOne(b => b.VeilingProduct)
            .WithMany(vp => vp.Bids)
            .HasForeignKey(b => b.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);


       
        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Koper)
            .WithMany(k => k.Bids)
            .HasForeignKey(b => b.KoperId)
            .OnDelete(DeleteBehavior.SetNull);



        modelBuilder.Entity<Veiling>()
            .HasMany(v => v.Bids)
            .WithOne(b => b.Veiling)
            .HasForeignKey(b => b.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);


    
        modelBuilder.Entity<AuditEntry>()
            .HasOne(a => a.Veiling)
            .WithMany(v => v.AuditEntries)
            .HasForeignKey(a => a.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);


    
        modelBuilder.Entity<AuditEntry>()
            .HasOne(a => a.ActorGebruiker)
            .WithMany()
            .HasForeignKey(a => a.ActorGebruikerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
