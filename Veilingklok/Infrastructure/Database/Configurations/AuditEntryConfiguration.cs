using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> b)
    {
        b.ToTable("AuditEntries");
        b.HasKey(x => x.Id);

        b.Property(x => x.Action).IsRequired().HasMaxLength(500);
        b.Property(x => x.CreatedAtUtc).IsRequired();

        b.HasOne(x => x.ActorGebruiker)
            .WithMany()
            .HasForeignKey(x => x.ActorGebruikerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.VeilingId, x.CreatedAtUtc });
    }
}