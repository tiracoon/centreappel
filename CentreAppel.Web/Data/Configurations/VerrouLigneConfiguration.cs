using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class VerrouLigneConfiguration : IEntityTypeConfiguration<VerrouLigneEntity>
{
    public void Configure(EntityTypeBuilder<VerrouLigneEntity> builder)
    {
        builder.HasKey(v => v.IdLCampagne);

        builder.HasOne(v => v.LigneCampagne)
            .WithMany()
            .HasForeignKey(v => v.IdLCampagne)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Operateur)
            .WithMany()
            .HasForeignKey(v => v.IdOperateur)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
