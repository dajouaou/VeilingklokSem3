using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.Property(b => b.Amount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.Source).HasConversion<string>();
        builder.Property(b => b.PlacedAtUtc).IsRequired();
        builder.Property(b => b.RowVersion).IsRowVersion();

        builder.HasOne(b => b.Koper)
            .WithMany(k => k.Bids)
            .HasForeignKey(b => b.KoperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.VeilingProduct)
            .WithMany(vp => vp.Bids)
            .HasForeignKey(b => b.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}