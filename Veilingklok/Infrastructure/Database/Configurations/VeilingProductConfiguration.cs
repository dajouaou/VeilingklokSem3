using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingProductConfiguration : IEntityTypeConfiguration<VeilingProduct>
{
    public void Configure(EntityTypeBuilder<VeilingProduct> builder)
    {
        builder.ToTable("VeilingProducten");
        builder.HasKey(vp => vp.Id);

        builder.HasOne(vp => vp.Aanmelding)
            .WithOne(a => a.VeilingProduct)
            .HasForeignKey<VeilingProduct>(vp => vp.AanmeldingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.MaximumPrijs).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinimumPrijs).HasColumnType("decimal(18,2)");
        builder.Property(x => x.HuidigePrijs).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DalingPerSeconde).HasColumnType("decimal(18,4)");

    }
}
