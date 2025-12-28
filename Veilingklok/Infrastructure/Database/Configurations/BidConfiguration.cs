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

        b.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        b.Property(x => x.PlacedAtUtc).IsRequired();

        b.HasIndex(x => new { x.VeilingId, x.VeilingProductId, x.PlacedAtUtc });
        b.HasIndex(x => new { x.KoperId, x.PlacedAtUtc });
    }
}