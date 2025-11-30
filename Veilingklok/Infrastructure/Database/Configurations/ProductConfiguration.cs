using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Producten");
        builder.HasKey(x => x.Id);

        // Kolommen (voorkomt nvarchar(max))
        builder.Property(x => x.Naam)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Categorie)
            .HasMaxLength(64);

        builder.Property(x => x.Beschrijving)
            .HasMaxLength(512);

        builder.Property(x => x.FotoUrl)
            .HasMaxLength(256);

        // Relatie
        builder.Property(x => x.AanvoerderId).IsRequired();

        builder.HasIndex(x => x.AanvoerderId); // handig voor joins/filters

        builder.HasOne(x => x.Aanvoerder)
            .WithMany(a => a.Producten)
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}