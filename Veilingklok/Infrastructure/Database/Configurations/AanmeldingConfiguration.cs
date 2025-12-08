using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

public class AanmeldingConfiguration : IEntityTypeConfiguration<Aanmelding>
{
    public void Configure(EntityTypeBuilder<Aanmelding> builder)
    {
        builder.ToTable("Aanmeldingen");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Soort)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Potmaat)
            .HasMaxLength(50);

        builder.Property(x => x.Steellengte)
            .HasMaxLength(50);

        builder.Property(x => x.MinimumPrijs)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Aanvoerder)
            .WithMany(a => a.Aanmeldingen)
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
