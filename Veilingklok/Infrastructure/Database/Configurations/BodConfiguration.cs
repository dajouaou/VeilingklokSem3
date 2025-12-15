using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

public class BodConfiguration : IEntityTypeConfiguration<Bod>
{
    public void Configure(EntityTypeBuilder<Bod> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.Veiling)
            .WithMany(v => v.Biedingen)
            .HasForeignKey(b => b.VeilingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.VeilingProduct)
            .WithMany(vp => vp.Biedingen)
            .HasForeignKey(b => b.VeilingProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
