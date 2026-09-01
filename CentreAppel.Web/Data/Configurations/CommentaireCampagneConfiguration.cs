using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class CommentaireCampagneConfiguration : IEntityTypeConfiguration<CommentaireCampagneEntity>
{
    public void Configure(EntityTypeBuilder<CommentaireCampagneEntity> builder)
    {
        builder.HasKey(c => c.IdCommentaire);
        builder.Property(c => c.Libelle).HasMaxLength(200).IsRequired();

        builder.HasOne(c => c.Campagne)
            .WithMany()
            .HasForeignKey(c => c.IdCampagne)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
