using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AanvoerderConfiguration : IEntityTypeConfiguration<Aanvoerder>
{
    public void Configure(EntityTypeBuilder<Aanvoerder> builder)
    {
        builder.ToTable("Aanvoerders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Naam)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.GebruikerId)
            .IsUnique();

        builder.HasOne(x => x.Gebruiker)
            .WithOne(g => g.Aanvoerder)
            .HasForeignKey<Aanvoerder>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict); // ✅ keep
    }
}