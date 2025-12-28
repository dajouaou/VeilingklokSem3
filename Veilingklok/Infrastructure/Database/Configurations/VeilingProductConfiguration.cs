using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingProductConfiguration : IEntityTypeConfiguration<VeilingProduct>
{
    public void Configure(EntityTypeBuilder<VeilingProduct> b)
    {
        b.ToTable("VeilingProducten");
        b.HasKey(x => x.Id);

        b.Property(x => x.Status).IsRequired();

        b.Property(x => x.StartPrijs).HasColumnType("decimal(18,2)");
        b.Property(x => x.HuidigePrijs).HasColumnType("decimal(18,2)");
        b.Property(x => x.MinimumPrijs).HasColumnType("decimal(18,2)");

        b.Property(x => x.DurationSeconds).IsRequired();
        b.Property(x => x.Hoeveelheid).IsRequired();
        b.Property(x => x.Volgorde).IsRequired();

       
        
        b.Property(x => x.RowVersion).IsRowVersion();

        
        
        
        b.HasOne(x => x.Aanmelding)
            .WithOne(x => x.VeilingProduct)
            .HasForeignKey<VeilingProduct>(x => x.AanmeldingId)
            .OnDelete(DeleteBehavior.Restrict);

      
        
        b.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Aanvoerder)
            .WithMany()
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Koper)
            .WithMany()
            .HasForeignKey(x => x.KoperId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasMany(x => x.Bids)
            .WithOne(x => x.VeilingProduct)
            .HasForeignKey(x => x.VeilingProductId)
            .OnDelete(DeleteBehavior.Cascade);

       
        
        
        b.HasIndex(x => new { x.VeilingId, x.Volgorde });
        b.HasIndex(x => new { x.VeilingId, x.Status });
    }
}
