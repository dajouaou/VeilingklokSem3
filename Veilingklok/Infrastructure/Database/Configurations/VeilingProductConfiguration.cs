using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VeilingProductConfiguration : IEntityTypeConfiguration<VeilingProduct>
{
    public void Configure(EntityTypeBuilder<VeilingProduct> builder)
    {
        builder.HasOne(vp => vp.Aanmelding)
               .WithOne(a => a.VeilingProduct)
               .HasForeignKey<VeilingProduct>(vp => vp.AanmeldingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
