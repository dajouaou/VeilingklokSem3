using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veilingklok.Core.Entities;

namespace Veilingklok.Infrastructure.Database.Configurations;

public sealed class GebruikerConfiguration : IEntityTypeConfiguration<Gebruiker>
{
    public void Configure(EntityTypeBuilder<Gebruiker> b)
    {
        b.ToTable("Gebruikers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Email).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(300);

        b.Property(x => x.Voornaam).IsRequired().HasMaxLength(100);
        b.Property(x => x.Achternaam).IsRequired().HasMaxLength(150);

        b.Property(x => x.Rol).IsRequired();
        b.Property(x => x.CreatedAtUtc).IsRequired();

       
        
        b.HasOne(x => x.Koper)
            .WithOne(x => x.Gebruiker)
            .HasForeignKey<Koper>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Aanvoerder)
            .WithOne(x => x.Gebruiker)
            .HasForeignKey<Aanvoerder>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.VM)
            .WithOne(x => x.Gebruiker)
            .HasForeignKey<VM>(x => x.GebruikerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}