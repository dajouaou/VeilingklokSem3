using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class AanmeldingConfiguration : IEntityTypeConfiguration<Aanmelding>
{
    public void Configure(EntityTypeBuilder<Aanmelding> b)
    {
        b.ToTable("Aanmeldingen");
        b.HasKey(x => x.Id);

        b.Property(x => x.Soort).IsRequired().HasMaxLength(200);
        b.Property(x => x.Potmaat).HasMaxLength(50);
        b.Property(x => x.Steellengte).HasMaxLength(50);

        b.Property(x => x.Hoeveelheid).IsRequired();
        b.Property(x => x.MinimumPrijs).HasColumnType("decimal(18,2)");

        b.Property(x => x.LeverDatum).IsRequired();
        b.Property(x => x.FotoUrl).HasMaxLength(500);
        b.Property(x => x.Beschrijving).HasMaxLength(1000);

      
        
        
        b.HasOne(x => x.VeilingProduct)
            .WithOne(x => x.Aanmelding)
            .HasForeignKey<Aanmelding>(x => x.VeilingProductId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => new { x.LeverDatum, x.VeilingProductId });
    }
}