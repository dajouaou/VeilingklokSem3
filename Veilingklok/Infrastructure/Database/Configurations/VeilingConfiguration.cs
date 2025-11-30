using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingConfiguration : IEntityTypeConfiguration<Veiling>
{
    public void Configure(EntityTypeBuilder<Veiling> builder)
    {
        builder.ToTable("Veilingen");
        builder.HasKey(v => v.Id);

        // Current lot pointer (optioneel)
        builder.Property(v => v.CurrentVeilingProductId)
            .IsRequired(false);

        builder.HasIndex(v => v.CurrentVeilingProductId);

        builder.HasMany(v => v.VeilingProducten)
            .WithOne(vp => vp.Veiling)
            .HasForeignKey(vp => vp.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);

        // verbreek multiple cascade paths
        builder.HasMany(v => v.Bids)
            .WithOne(b => b.Veiling)
            .HasForeignKey(b => b.VeilingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(v => v.AuditEntries)
            .WithOne(a => a.Veiling)
            .HasForeignKey(a => a.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Current lot pointer: NO ACTION (NIET SetNull)
        builder.HasOne(v => v.CurrentVeilingProduct)
            .WithMany()
            .HasForeignKey(v => v.CurrentVeilingProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}