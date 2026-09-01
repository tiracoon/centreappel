using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class LigneCampagneConfiguration : IEntityTypeConfiguration<LigneCampagneEntity>
{
    public void Configure(EntityTypeBuilder<LigneCampagneEntity> builder)
    {
        builder.ToTable("l_campagnes");
        builder.HasKey(l => l.IdLCampagne);

        builder.Property(l => l.CodeSoc).HasColumnType("character(3)").IsRequired();
        builder.Property(l => l.NumCli).HasPrecision(12, 0).IsRequired();

        builder.Property(l => l.Siret).HasMaxLength(14);
        builder.Property(l => l.RaisonSociale).HasMaxLength(120);
        builder.Property(l => l.SousActivite).HasMaxLength(60);
        builder.Property(l => l.MagasinAffilie).HasMaxLength(60);
        builder.Property(l => l.Correspondant).HasMaxLength(80);
        builder.Property(l => l.Telephone).HasMaxLength(25);
        builder.Property(l => l.Email).HasMaxLength(120);
        builder.Property(l => l.Adresse).HasMaxLength(200);
        builder.Property(l => l.Cp).HasMaxLength(10);
        builder.Property(l => l.Ville).HasMaxLength(60);
        builder.Property(l => l.Pays).HasMaxLength(60);
        builder.Property(l => l.Langue).HasMaxLength(20);
        builder.Property(l => l.Rfm).HasMaxLength(10);
        builder.Property(l => l.CaHt).HasPrecision(14, 2);

        builder.HasOne(l => l.Campagne)
            .WithMany()
            .HasForeignKey(l => l.IdCampagne)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.OperateurAssigne)
            .WithMany()
            .HasForeignKey(l => l.IdOperateurAssigne)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne<OperateurEntity>()
            .WithMany()
            .HasForeignKey(l => l.IdOperateurCm)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Anti-doublon : un client ne peut apparaître qu'une fois par campagne.
        builder.HasIndex(l => new { l.IdCampagne, l.CodeSoc, l.NumCli }).IsUnique();
    }
}
