using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeildagConfiguration : IEntityTypeConfiguration<Veildag>
{
    public void Configure(EntityTypeBuilder<Veildag> b)
    {
        b.ToTable("Veildagen");
        b.HasKey(x => x.Id);
        b.Property(x => x.Datum).IsRequired();
        b.HasIndex(x => x.Datum).IsUnique();
    }
}