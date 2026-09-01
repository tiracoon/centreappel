using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class OperateurConfiguration : IEntityTypeConfiguration<OperateurEntity>
{
    public void Configure(EntityTypeBuilder<OperateurEntity> builder)
    {
        builder.HasKey(o => o.IdOperateur);
        builder.HasIndex(o => o.LoginAd).IsUnique();

        builder.HasOne(o => o.Role)
            .WithMany()
            .HasForeignKey(o => o.IdRole)
            .IsRequired();
    }
}
