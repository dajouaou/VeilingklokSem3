using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Producten");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MinimumPrijs)
            .HasColumnType("decimal(18,2)"); // geld
    }
}
