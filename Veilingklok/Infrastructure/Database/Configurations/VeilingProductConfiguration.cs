using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class VeilingProductConfiguration : IEntityTypeConfiguration<VeilingProduct>
{
    public void Configure(EntityTypeBuilder<VeilingProduct> builder)
    {
        builder.ToTable("VeilingProducten");
        builder.HasKey(vp => vp.Id);

        builder.HasOne(vp => vp.Aanmelding)
            .WithOne(a => a.VeilingProduct)
            .HasForeignKey<VeilingProduct>(vp => vp.AanmeldingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
