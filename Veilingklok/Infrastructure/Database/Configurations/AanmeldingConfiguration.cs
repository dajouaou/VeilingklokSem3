using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AanmeldingConfiguration : IEntityTypeConfiguration<Aanmelding>
{
    // Configuratie voor hoe de Aanmelding-entity in de database wordt opgeslagen
    public void Configure(EntityTypeBuilder<Aanmelding> builder)
    {
        // Zet de tabelnaam en primary key
        builder.ToTable("Aanmeldingen");
        builder.HasKey(x => x.Id);

        // Stelt regels in voor het Soort-veld
        builder.Property(x => x.Soort)
            .HasMaxLength(100)
            .IsRequired();

        // Stelt max lengte in voor Potmaat
        builder.Property(x => x.Potmaat)
            .HasMaxLength(50);

        // Stelt max lengte in voor Steellengte
        builder.Property(x => x.Steellengte)
            .HasMaxLength(50);

        // Stelt precisie in voor MinimumPrijs
        builder.Property(x => x.MinimumPrijs)
            .HasPrecision(18, 2);

        // Stelt LeverDatum in als verplicht en alleen datum (geen tijd)
        builder.Property(x => x.LeverDatum)
            .IsRequired()
            .HasColumnType("date");

        // Configureert de relatie met Aanvoerder en het delete-gedrag
        builder.HasOne(x => x.Aanvoerder)
            .WithMany(a => a.Aanmeldingen)
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
