using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AanvoerderConfiguration : IEntityTypeConfiguration<Aanvoerder>
{
    public void Configure(EntityTypeBuilder<Aanvoerder> b)
    {
        b.ToTable("Aanvoerders");
        b.HasKey(x => x.Id);

        b.Property(x => x.Naam).IsRequired().HasMaxLength(200);

        b.HasIndex(x => x.GebruikerId).IsUnique();

        b.HasMany(x => x.Aanmeldingen)
            .WithOne(x => x.Aanvoerder)
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}