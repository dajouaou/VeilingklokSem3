using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class BodConfiguration : IEntityTypeConfiguration<Bod>
{
    public void Configure(EntityTypeBuilder<Bod> builder)
    {
        builder.ToTable("Biedingen");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Prijs)
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Tijdstip)
            .HasColumnType("datetime2");

        builder.HasOne(b => b.Veiling)
            .WithMany(v => v.Biedingen)
            .HasForeignKey(b => b.VeilingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.VeilingProduct)
            .WithMany(vp => vp.Biedingen)
            .HasForeignKey(b => b.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
