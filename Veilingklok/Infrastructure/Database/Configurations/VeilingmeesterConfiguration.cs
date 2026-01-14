using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations
{
    public class VeilingmeesterConfiguration : IEntityTypeConfiguration<Veilingmeester>
    {
        // Configuratie voor hoe de Veilingmeester-entity in de database wordt opgeslagen
        public void Configure(EntityTypeBuilder<Veilingmeester> builder)
        {
            // Zet de tabelnaam
            builder.ToTable("Veilingmeesters");

            // Stelt de primary key in
            builder.HasKey(x => x.Id);

            // Regels voor de Naam-kolom
            builder.Property(x => x.Naam)
                .HasMaxLength(128)
                .IsRequired();

            // Zorgt dat elke gebruiker maar één veilingmeester-profiel kan hebben
            builder.HasIndex(x => x.GebruikerId)
                .IsUnique();

            // Configureert de 1-op-1 relatie met Gebruiker
            builder.HasOne(x => x.Gebruiker)
                .WithOne(g => g.Veilingmeester)
                .HasForeignKey<Veilingmeester>(x => x.GebruikerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
