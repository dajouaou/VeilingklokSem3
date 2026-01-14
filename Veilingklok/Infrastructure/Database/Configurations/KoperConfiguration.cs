using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class KoperConfiguration : IEntityTypeConfiguration<Koper>
{
    // Configuratie voor hoe de Koper-entity in de database wordt opgeslagen
    public void Configure(EntityTypeBuilder<Koper> builder)
    {
        // Zet de tabelnaam
        builder.ToTable("Kopers");

        // Stelt de primary key in
        builder.HasKey(x => x.Id);

        // Regels voor de Naam-kolom
        builder.Property(x => x.Naam)
            .HasMaxLength(128)
            .IsRequired();

        // Zorgt dat elke gebruiker maar één koper-profiel kan hebben
        builder.HasIndex(x => x.GebruikerId).IsUnique();

        // Configureert de 1-op-1 relatie met Gebruiker
        builder.HasOne(x => x.Gebruiker)
            .WithOne(g => g.Koper)
            .HasForeignKey<Koper>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
