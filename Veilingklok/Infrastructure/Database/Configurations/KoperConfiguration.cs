using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class KoperConfiguration : IEntityTypeConfiguration<Koper>
{
    public void Configure(EntityTypeBuilder<Koper> builder)
    {
        builder.ToTable("Kopers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Naam)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.GebruikerId).IsUnique();

        builder.HasOne(x => x.Gebruiker)
            .WithOne(g => g.Koper)
            .HasForeignKey<Koper>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}