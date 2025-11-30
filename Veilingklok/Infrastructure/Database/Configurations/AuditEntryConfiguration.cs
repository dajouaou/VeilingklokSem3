using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable("AuditEntries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.VeilingId);
        builder.HasIndex(x => x.ActorGebruikerId);

        builder.HasOne(x => x.Veiling)
            .WithMany(v => v.AuditEntries)
            .HasForeignKey(x => x.VeilingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ActorGebruiker)
            .WithMany()
            .HasForeignKey(x => x.ActorGebruikerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}