using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class TransactieConfiguration : IEntityTypeConfiguration<Transactie>
{
    public void Configure(EntityTypeBuilder<Transactie> builder)
    {
        builder.ToTable("Transacties");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Prijs)
            .HasColumnType("decimal(18,2)");   // ✅ geld

        builder.Property(t => t.Tijdstip)
            .HasColumnType("datetime2");

        // (optioneel maar vaak nodig)
        builder.HasOne(t => t.VeilingProduct)
            .WithMany(vp => vp.Transacties)
            .HasForeignKey(t => t.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Als Transactie.KoperId naar Gebruiker.Id wijst:
        builder.HasOne(t => t.Koper)
            .WithMany()
            .HasForeignKey(t => t.KoperId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
