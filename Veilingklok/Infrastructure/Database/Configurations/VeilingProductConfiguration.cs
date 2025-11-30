using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingProductConfiguration : IEntityTypeConfiguration<VeilingProduct>
{
    public void Configure(EntityTypeBuilder<VeilingProduct> builder)
    {
        builder.ToTable("VeilingProducten");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartPrijs).HasPrecision(18, 2);
        builder.Property(x => x.HuidigePrijs).HasPrecision(18, 2);

        builder.HasOne(x => x.Veiling)
            .WithMany(v => v.VeilingProducten)
            .HasForeignKey(x => x.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.VeilingProducten)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        //  nieuw
        builder.HasOne(x => x.Aanvoerder)
            .WithMany(a => a.VeilingProducten)
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SoldToKoper)
            .WithMany(k => k.GekochteVeilingProducten)
            .HasForeignKey(x => x.SoldToKoperId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.VeilingId, x.Volgorde }).IsUnique();
    }
}
