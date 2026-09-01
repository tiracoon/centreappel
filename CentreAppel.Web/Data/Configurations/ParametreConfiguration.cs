using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class ParametreConfiguration : IEntityTypeConfiguration<ParametreEntity>
{
    public void Configure(EntityTypeBuilder<ParametreEntity> builder)
    {
        builder.HasKey(p => p.IdParametre);
        builder.Property(p => p.Libelle).HasMaxLength(60).IsRequired();
        builder.Property(p => p.ValeurTexte).HasMaxLength(200);
        builder.Property(p => p.ValeurNum).HasPrecision(12, 2);

        builder.HasIndex(p => p.Libelle).IsUnique();
    }
}
