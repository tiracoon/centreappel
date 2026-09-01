using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class ActionCampagneConfiguration : IEntityTypeConfiguration<ActionCampagneEntity>
{
    public void Configure(EntityTypeBuilder<ActionCampagneEntity> builder)
    {
        builder.HasKey(a => a.IdActionsCampagnes);
        builder.Property(a => a.CommentaireLibre).HasColumnType("text");

        builder.HasOne(a => a.LigneCampagne)
            .WithMany()
            .HasForeignKey(a => a.IdLCampagne)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Operateur)
            .WithMany()
            .HasForeignKey(a => a.IdOperateur)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TypeContact)
            .WithMany()
            .HasForeignKey(a => a.IdTypeContact)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Deroulement)
            .WithMany()
            .HasForeignKey(a => a.IdDeroulement)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Interet)
            .WithMany()
            .HasForeignKey(a => a.IdInteret)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Canal)
            .WithMany()
            .HasForeignKey(a => a.IdCanal)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Commentaire)
            .WithMany()
            .HasForeignKey(a => a.IdCommentaire)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OperateurEntity>()
            .WithMany()
            .HasForeignKey(a => a.IdOperateurCm)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(a => new { a.IdLCampagne, a.NumAction }).IsUnique();
        builder.HasIndex(a => a.DateRelance).HasFilter("date_relance IS NOT NULL");
    }
}
