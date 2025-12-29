using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingConfiguration : IEntityTypeConfiguration<Veiling>
{
    public void Configure(EntityTypeBuilder<Veiling> b)
    {
        b.ToTable("Veilingen");
        b.HasKey(x => x.Id);

        b.Property(x => x.Naam).HasMaxLength(200);
        b.Property(x => x.Datum).IsRequired();
        b.Property(x => x.StartTijd).IsRequired();
        b.Property(x => x.Status).IsRequired();

        b.HasMany(x => x.VeilingProducten)
            .WithOne(x => x.Veiling)
            .HasForeignKey(x => x.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.AuditEntries)
            .WithOne(x => x.Veiling)
            .HasForeignKey(x => x.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.CurrentVeilingProduct)
            .WithMany()
            .HasForeignKey(x => x.CurrentVeilingProductId)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasIndex(x => new { x.Datum, x.StartTijd });
        b.HasIndex(x => x.Status);
    }
}