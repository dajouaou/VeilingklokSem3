using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations
{
    public class VeilingConfiguration : IEntityTypeConfiguration<Veiling>
    {
        public void Configure(EntityTypeBuilder<Veiling> builder)
        {
            // 1 -op- veel relatie met VeilingProduct
            builder
                .HasMany(v => v.Producten)
                .WithOne(p => p.Veiling)
                .HasForeignKey(p => p.VeilingId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1 -op- 1 relatie met huidig product
            builder
                .HasOne(v => v.HuidigProduct)
                .WithMany() // belangrijk: geen back-reference
                .HasForeignKey(v => v.HuidigProductId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(v => v.Datum)
    .HasColumnType("date");

            builder.Property(v => v.StartTijd)
                .HasColumnType("time");

        }
    }
}
