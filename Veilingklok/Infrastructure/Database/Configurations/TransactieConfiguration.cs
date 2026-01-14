using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class TransactieConfiguration : IEntityTypeConfiguration<Transactie>
{
    // Configuratie voor hoe transacties in de database worden opgeslagen
    public void Configure(EntityTypeBuilder<Transactie> builder)
    {
        // Zet tabelnaam en primary key
        builder.ToTable("Transacties");
        builder.HasKey(t => t.Id);

        // Stelt type en precisie in voor prijs
        builder.Property(t => t.Prijs)
            .HasColumnType("decimal(18,2)");

        // Stelt type in voor tijdstip
        builder.Property(t => t.Tijdstip)
            .HasColumnType("datetime2");

        // Relatie: transactie hoort bij één veilingproduct
        builder.HasOne(t => t.VeilingProduct)
            .WithMany(vp => vp.Transacties)
            .HasForeignKey(t => t.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relatie: transactie hoort bij één koper (gebruiker)
        builder.HasOne(t => t.Koper)
            .WithMany()
            .HasForeignKey(t => t.KoperId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
