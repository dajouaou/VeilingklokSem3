using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.ToTable("Biedingen");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PlacedAtUtc)
            .IsRequired();

        // handige indexes
        builder.HasIndex(x => x.VeilingId);
        builder.HasIndex(x => x.VeilingProductId);
        builder.HasIndex(x => x.PlacedByGebruikerId);
        builder.HasIndex(x => x.KoperId);

        //  multiple cascade paths voorkomen:
        // cascade loopt via Veiling -> VeilingProduct -> Biedingen
        builder.HasOne(x => x.Veiling)
            .WithMany(v => v.Bids)
            .HasForeignKey(x => x.VeilingId)
            .OnDelete(DeleteBehavior.NoAction); // of Restrict

        builder.HasOne(x => x.VeilingProduct)
            .WithMany(vp => vp.Bids)
            .HasForeignKey(x => x.VeilingProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PlacedByGebruiker)
            .WithMany()
            .HasForeignKey(x => x.PlacedByGebruikerId)
            .OnDelete(DeleteBehavior.Restrict); // of NoAction

        builder.HasOne(x => x.Koper)
            .WithMany(k => k.Bids)
            .HasForeignKey(x => x.KoperId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}