using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AanvoerderConfiguration : IEntityTypeConfiguration<Aanvoerder>
{
    // Configuratie voor hoe de Aanvoerder-entity in de database wordt opgeslagen
    public void Configure(EntityTypeBuilder<Aanvoerder> builder)
    {
        // Zet de tabelnaam
        builder.ToTable("Aanvoerders");

        // Stelt de primary key in
        builder.HasKey(x => x.Id);

        // Regels voor de Naam-kolom
        builder.Property(x => x.Naam)
            .HasMaxLength(128)
            .IsRequired();

        // Zorgt dat elke gebruiker maar één aanvoerder-profiel kan hebben
        builder.HasIndex(x => x.GebruikerId)
            .IsUnique();

        // Configureert de 1-op-1 relatie met Gebruiker
        builder.HasOne(x => x.Gebruiker)
            .WithOne(g => g.Aanvoerder)
            .HasForeignKey<Aanvoerder>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict); // voorkomt automatisch verwijderen
    }
}
