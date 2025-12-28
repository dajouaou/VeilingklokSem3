using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingmeesterConfiguration : IEntityTypeConfiguration<VM>
{
    public void Configure(EntityTypeBuilder<VM> b)
    {
        b.ToTable("Veilingmeesters");
        b.HasKey(x => x.Id);

        b.Property(x => x.Naam).IsRequired().HasMaxLength(200);

        b.HasIndex(x => x.GebruikerId).IsUnique();

        b.HasMany(x => x.Veilingen)
            .WithOne(x => x.VM)
            .HasForeignKey(x => x.VMId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}