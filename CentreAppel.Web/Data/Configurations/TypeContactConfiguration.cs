using CentreAppel.Web.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentreAppel.Web.Data.Configurations;

public class TypeContactConfiguration : IEntityTypeConfiguration<TypeContactEntity>
{
    public void Configure(EntityTypeBuilder<TypeContactEntity> builder)
    {
        builder.HasKey(t => t.IdTypeContact);
        builder.Property(t => t.Code).HasMaxLength(40).IsRequired();
        builder.HasIndex(t => t.Code).IsUnique();

        // Une seule valeur peut être marquée par défaut.
        builder.HasIndex(t => t.Defaut).IsUnique().HasFilter("defaut = true");
    }
}
