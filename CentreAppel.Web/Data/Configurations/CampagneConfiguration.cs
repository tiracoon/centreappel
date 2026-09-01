using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class CampagneConfiguration : IEntityTypeConfiguration<CampagneEntity>
{
    public void Configure(EntityTypeBuilder<CampagneEntity> builder)
    {
        builder.ToTable("e_campagnes");
        builder.HasKey(c => c.IdCampagne);

        builder.Property(c => c.Nom).HasMaxLength(120).IsRequired();
        builder.Property(c => c.Description).IsRequired();

        builder.HasOne<OperateurEntity>()
            .WithMany()
            .HasForeignKey(c => c.IdOperateurCm)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
