using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

public class VeilingmeesterConfiguration : IEntityTypeConfiguration<Veilingmeester>
{
    public void Configure(EntityTypeBuilder<Veilingmeester> builder)
    {
        builder.ToTable("Veilingmeesters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Naam)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.GebruikerId).IsUnique();

        builder.HasOne(x => x.Gebruiker)
            .WithOne(g => g.Veilingmeester)
            .HasForeignKey<Veilingmeester>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
