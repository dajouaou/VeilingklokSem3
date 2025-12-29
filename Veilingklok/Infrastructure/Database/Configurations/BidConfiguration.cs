using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class BodConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> b)
    {
        b.ToTable("Bids");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
        b.Property(x => x.PlacedAtUtc).IsRequired();

        b.HasOne(x => x.Koper)
            .WithMany(x => x.Bids)
            .HasForeignKey(x => x.KoperId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.VeilingProduct)
            .WithMany()
            .HasForeignKey(x => x.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Veiling)
            .WithMany()
            .HasForeignKey(x => x.VeilingId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.VeilingId, x.VeilingProductId, x.PlacedAtUtc });
        b.HasIndex(x => new { x.KoperId, x.PlacedAtUtc });
    }
}