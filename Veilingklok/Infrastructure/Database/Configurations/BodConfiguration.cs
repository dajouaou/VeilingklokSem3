using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class BodConfiguration : IEntityTypeConfiguration<Bod>
{
    // Configuratie voor hoe biedingen in de database worden opgeslagen
    public void Configure(EntityTypeBuilder<Bod> builder)
    {
        // Zet tabelnaam en primary key
        builder.ToTable("Biedingen");
        builder.HasKey(b => b.Id);

        // Stelt type en precisie in voor prijs
        builder.Property(b => b.Prijs)
            .HasColumnType("decimal(18,2)");

        // Stelt type in voor tijdstip
        builder.Property(b => b.Tijdstip)
            .HasColumnType("datetime2");

        // Relatie: bod hoort bij één veiling
        builder.HasOne(b => b.Veiling)
            .WithMany(v => v.Biedingen)
            .HasForeignKey(b => b.VeilingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relatie: bod hoort bij één veilingproduct
        builder.HasOne(b => b.VeilingProduct)
            .WithMany(vp => vp.Biedingen)
            .HasForeignKey(b => b.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
