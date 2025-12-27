using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations
{
    public class VeilingmeesterConfiguration : IEntityTypeConfiguration<VM>
    {
        public void Configure(EntityTypeBuilder<VM> builder)
        {
            builder.ToTable("Veilingmeesters");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Naam)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasIndex(x => x.GebruikerId)
                .IsUnique();

            builder.HasOne(x => x.Gebruiker)
                .WithOne(g => g.VM)
                .HasForeignKey<VM>(x => x.GebruikerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
