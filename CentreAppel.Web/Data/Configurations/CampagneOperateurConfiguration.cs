using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class CampagneOperateurConfiguration : IEntityTypeConfiguration<CampagneOperateurEntity>
{
    public void Configure(EntityTypeBuilder<CampagneOperateurEntity> builder)
    {
        builder.HasKey(co => co.IdCampagneOperateur);

        builder.HasOne(co => co.Campagne)
            .WithMany()
            .HasForeignKey(co => co.IdCampagne)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(co => co.Operateur)
            .WithMany()
            .HasForeignKey(co => co.IdOperateur)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(co => new { co.IdCampagne, co.IdOperateur }).IsUnique();
    }
}
