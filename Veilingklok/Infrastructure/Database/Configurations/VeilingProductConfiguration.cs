using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingProductConfiguration : IEntityTypeConfiguration<VeilingProduct>
{
    // Configuratie voor hoe VeilingProduct in de database wordt opgeslagen
    public void Configure(EntityTypeBuilder<VeilingProduct> builder)
    {
        // Zet tabelnaam en primary key
        builder.ToTable("VeilingProducten");
        builder.HasKey(vp => vp.Id);

        // Configureert 1-op-1 relatie met Aanmelding
        builder.HasOne(vp => vp.Aanmelding)
            .WithOne(a => a.VeilingProduct)
            .HasForeignKey<VeilingProduct>(vp => vp.AanmeldingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Stelt types in voor geld- en prijsvelden
        builder.Property(x => x.MaximumPrijs).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinimumPrijs).HasColumnType("decimal(18,2)");
        builder.Property(x => x.HuidigePrijs).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DalingPerSeconde).HasColumnType("decimal(18,4)");
    }
}
