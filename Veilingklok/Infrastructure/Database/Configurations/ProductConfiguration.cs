using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Producten");
        b.HasKey(x => x.Id);

        b.Property(x => x.Naam).IsRequired().HasMaxLength(200);
        b.Property(x => x.Categorie).HasMaxLength(100);
        b.Property(x => x.Beschrijving).HasMaxLength(1000);
        b.Property(x => x.FotoUrl).HasMaxLength(500);

        b.Property(x => x.Soort).IsRequired().HasMaxLength(200);
        b.Property(x => x.PotmaatOfSteellengte).HasMaxLength(50);
        b.Property(x => x.MinimumPrijs).HasColumnType("decimal(18,2)");

        b.Property(x => x.KlokLocatie).HasMaxLength(50);

        b.HasOne(x => x.Aanvoerder)
            .WithMany()
            .HasForeignKey(x => x.AanvoerderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}